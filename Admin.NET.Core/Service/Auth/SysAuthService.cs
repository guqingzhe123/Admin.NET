// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Furion.SpecificationDocument;
using Lazy.Captcha.Core;

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统登录授权服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 500)]
public class SysAuthService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysUser> _sysUserRep;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SysMenuService _sysMenuService;
    private readonly SysOnlineUserService _sysOnlineUserService;
    private readonly SysConfigService _sysConfigService;
    private readonly ICaptcha _captcha;
    private readonly SysCacheService _sysCacheService;

    public SysAuthService(UserManager userManager,
        SqlSugarRepository<SysUser> sysUserRep,
        IHttpContextAccessor httpContextAccessor,
        SysMenuService sysMenuService,
        SysOnlineUserService sysOnlineUserService,
        SysConfigService sysConfigService,
        ICaptcha captcha,
        SysCacheService sysCacheService)
    {
        _userManager = userManager;
        _sysUserRep = sysUserRep;
        _httpContextAccessor = httpContextAccessor;
        _sysMenuService = sysMenuService;
        _sysOnlineUserService = sysOnlineUserService;
        _sysConfigService = sysConfigService;
        _captcha = captcha;
        _sysCacheService = sysCacheService;
    }

    /// <summary>
    /// 账号密码登录 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <remarks>用户名/密码：superadmin/123456</remarks>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("账号密码登录")]
    public virtual async Task<LoginOutput> Login([Required] LoginInput input)
    {
        //// 可以根据域名获取具体租户
        //var host = _httpContextAccessor.HttpContext.Request.Host;

        // 判断密码错误次数（缓存30分钟）
        var keyPasswordErrorTimes = $"{CacheConst.KeyPasswordErrorTimes}{input.Account}";
        var passwordErrorTimes = _sysCacheService.Get<int>(keyPasswordErrorTimes);
        var passwordMaxErrorTimes = await _sysConfigService.GetConfigValue<int>(ConfigConst.SysPasswordMaxErrorTimes);
        // 若未配置或误配置为0、负数, 则默认密码错误次数最大为10次
        if (passwordMaxErrorTimes < 1) passwordMaxErrorTimes = 10;
        if (passwordErrorTimes > passwordMaxErrorTimes) throw Oops.Oh(ErrorCodeEnum.D1027);

        // 判断是否开启验证码，其校验验证码
       // if (await _sysConfigService.GetConfigValue<bool>(ConfigConst.SysCaptcha) && !_captcha.Validate(input.CodeId.ToString(), input.Code)) throw Oops.Oh(ErrorCodeEnum.D0008);
        
        // 获取登录租户和用户
        var (tenant, user) = await GetLoginUserAndTenant(input.Host, account: input.Account);

        // 账号是否被冻结
        if (user.Status == StatusEnum.Disable) throw Oops.Oh(ErrorCodeEnum.D1017);

        // 是否开启域登录验证
        if (await _sysConfigService.GetConfigValue<bool>(ConfigConst.SysDomainLogin))
        {
            var userLdap = await _sysUserRep.ChangeRepository<SqlSugarRepository<SysUserLdap>>().GetFirstAsync(u => u.UserId == user.Id && u.TenantId == tenant.Id);
            if (userLdap == null)
            {
                VerifyPassword(input.Password, keyPasswordErrorTimes, passwordErrorTimes, user);
            }
            else if (!await App.GetRequiredService<SysLdapService>().AuthAccount(tenant.Id, userLdap.Account, CryptogramUtil.Decrypt(input.Password)))
            {
                _sysCacheService.Set(keyPasswordErrorTimes, ++passwordErrorTimes, TimeSpan.FromMinutes(30));
                throw Oops.Oh(ErrorCodeEnum.D1000);
            }
        }
        else
            VerifyPassword(input.Password, keyPasswordErrorTimes, passwordErrorTimes, user);

        // 登录成功则清空密码错误次数
        _sysCacheService.Remove(keyPasswordErrorTimes);

        return await CreateToken(user, tenant.AppId);
    }
    
    /// <summary>
    /// 获取登录租户和用户
    /// </summary>
    /// <param name="host"></param>
    /// <param name="account"></param>
    /// <param name="phone"></param>
    /// <returns></returns>
    [NonAction]
    public async Task<(SysTenant tenant, SysUser user)> GetLoginUserAndTenant(string host, string account=null, string phone=null)
    {
        // 是否租户隔离登录验证
        var isTenantHostLogin = await _sysConfigService.GetConfigValue<bool>(ConfigConst.SysTenantHostLogin);
        
        SysUser user;
        SysTenant tenant;
        
        // 租户隔离登录
        if (isTenantHostLogin)
        {
            // 若租户域名为空或为本地域名，则取默认租户域名
            if (string.IsNullOrWhiteSpace(host) || host.StartsWith("localhost")) host = SqlSugarConst.DefaultTenantHost;
            
            // 租户是否存在或已禁用
            tenant = await _sysUserRep.ChangeRepository<SqlSugarRepository<SysTenant>>().GetFirstAsync(u => u.Host == host.ToLower());
            if (tenant?.Status != StatusEnum.Enable) throw Oops.Oh(ErrorCodeEnum.Z1003);

            // 根据入参类型、租户查询登录用户
            user = await _sysUserRep.AsQueryable().Includes(t => t.SysOrg).ClearFilter()
                .Where(u => u.TenantId == tenant.Id || u.AccountType == AccountTypeEnum.SuperAdmin)
                .WhereIF(!string.IsNullOrWhiteSpace(account), u => u.Account.Equals(account))
                .WhereIF(!string.IsNullOrWhiteSpace(phone), u => u.Phone.Equals(phone))
                .FirstAsync();
            _ = user ?? throw Oops.Oh(ErrorCodeEnum.D0009);
            
            // 若登录的是超级管理员，则引用当前绑定的租户，这样登陆后操作的租户数据会与该租户关联
            if (user.AccountType == AccountTypeEnum.SuperAdmin) user.TenantId = tenant.Id;
        }
        else
        {
            // 根据入参类型查询登录用户
            user = await _sysUserRep.AsQueryable().Includes(t => t.SysOrg).ClearFilter()
                .WhereIF(!string.IsNullOrWhiteSpace(account), u => u.Account.Equals(account))
                .WhereIF(!string.IsNullOrWhiteSpace(phone), u => u.Phone.Equals(phone))
                .FirstAsync();
            _ = user ?? throw Oops.Oh(ErrorCodeEnum.D0009);
            
            // 租户是否存在或已禁用
            tenant = await _sysUserRep.ChangeRepository<SqlSugarRepository<SysTenant>>().GetFirstAsync(u => u.Id == user.TenantId);
            if (tenant?.Status != StatusEnum.Enable) throw Oops.Oh(ErrorCodeEnum.Z1003);
        }

        return (tenant, user);
    }

    /// <summary>
    /// 验证用户密码
    /// </summary>
    /// <param name="password"></param>
    /// <param name="keyPasswordErrorTimes"></param>
    /// <param name="passwordErrorTimes"></param>
    /// <param name="user"></param>
    private void VerifyPassword(string password, string keyPasswordErrorTimes, int passwordErrorTimes, SysUser user)
    {
        if (CryptogramUtil.CryptoType == CryptogramEnum.MD5.ToString())
        {
            if (user.Password.Equals(MD5Encryption.Encrypt(password))) return;
            
            _sysCacheService.Set(keyPasswordErrorTimes, ++passwordErrorTimes, TimeSpan.FromMinutes(30));
            throw Oops.Oh(ErrorCodeEnum.D1000);
        }

        // 国密SM2解密（前端密码传输SM2加密后的）
        password = CryptogramUtil.SM2Decrypt(password);
        if (CryptogramUtil.Decrypt(user.Password).Equals(password)) return;

        _sysCacheService.Set(keyPasswordErrorTimes, ++passwordErrorTimes, TimeSpan.FromMinutes(30));
        throw Oops.Oh(ErrorCodeEnum.D1000);
    }

    /// <summary>
    /// 验证锁屏密码 🔖
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    [DisplayName("验证锁屏密码")]
    public virtual async Task<bool> UnLockScreen([Required, FromQuery] string password)
    {
        // 账号是否存在
        var user = await _sysUserRep.GetFirstAsync(u => u.Id == _userManager.UserId);
        _ = user ?? throw Oops.Oh(ErrorCodeEnum.D0009);

        var keyPasswordErrorTimes = $"{CacheConst.KeyPasswordErrorTimes}{user.Account}";
        var passwordErrorTimes = _sysCacheService.Get<int>(keyPasswordErrorTimes);

        // 是否开启域登录验证
        if (await _sysConfigService.GetConfigValue<bool>(ConfigConst.SysDomainLogin))
        {
            var userLdap = await _sysUserRep.ChangeRepository<SqlSugarRepository<SysUserLdap>>().GetFirstAsync(u => u.UserId == user.Id && u.TenantId == user.TenantId);
            if (userLdap == null)
            {
                VerifyPassword(password, keyPasswordErrorTimes, passwordErrorTimes, user);
            }
            else if (!await App.GetRequiredService<SysLdapService>().AuthAccount(user.TenantId.Value, userLdap.Account, CryptogramUtil.Decrypt(password)))
            {
                _sysCacheService.Set(keyPasswordErrorTimes, ++passwordErrorTimes, TimeSpan.FromMinutes(30));
                throw Oops.Oh(ErrorCodeEnum.D1000);
            }
        }
        else
            VerifyPassword(password, keyPasswordErrorTimes, passwordErrorTimes, user);

        return true;
    }

    /// <summary>
    /// 手机号登录 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("手机号登录")]
    public virtual async Task<LoginOutput> LoginPhone([Required] LoginPhoneInput input)
    {
        // 校验短信验证码
        App.GetRequiredService<SysSmsService>().VerifyCode(new SmsVerifyCodeInput { Phone = input.Phone, Code = input.Code });
        
        // 获取登录租户和用户
        var (tenant, user) = await GetLoginUserAndTenant(input.Host, phone: input.Phone);

        return await CreateToken(user, tenant.AppId);
    }

    /// <summary>
    /// 生成Token令牌 🔖
    /// </summary>
    /// <param name="user"></param>
    /// <param name="appId"></param>
    /// <returns></returns>
    [NonAction]
    internal virtual async Task<LoginOutput> CreateToken(SysUser user, long appId)
    {
        // 单用户登录
        await _sysOnlineUserService.SingleLogin(user.Id);

        // 生成Token令牌
        var tokenExpire = await _sysConfigService.GetTokenExpire();
        var accessToken = JWTEncryption.Encrypt(new Dictionary<string, object>
        {
            { ClaimConst.AppId, appId },
            { ClaimConst.UserId, user.Id },
            { ClaimConst.TenantId, user.TenantId },
            { ClaimConst.Account, user.Account },
            { ClaimConst.RealName, user.RealName },
            { ClaimConst.AccountType, user.AccountType },
            { ClaimConst.OrgId, user.OrgId },
            { ClaimConst.OrgName, user.SysOrg?.Name },
            { ClaimConst.OrgType, user.SysOrg?.Type },
        }, tokenExpire);

        // 生成刷新Token令牌
        var refreshTokenExpire = await _sysConfigService.GetRefreshTokenExpire();
        var refreshToken = JWTEncryption.GenerateRefreshToken(accessToken, refreshTokenExpire);

        // 设置响应报文头
        _httpContextAccessor.HttpContext.SetTokensOfResponseHeaders(accessToken, refreshToken);

        // Swagger Knife4UI-AfterScript登录脚本
        // ke.global.setAllHeader('Authorization', 'Bearer ' + ke.response.headers['access-token']);

        // 更新用户登录信息
        user.LastLoginIp = _httpContextAccessor.HttpContext.GetRemoteIpAddressToIPv4(true);
        (user.LastLoginAddress, double? longitude, double? latitude) = CommonUtil.GetIpAddress(user.LastLoginIp);
        user.LastLoginTime = DateTime.Now;
        user.LastLoginDevice = CommonUtil.GetClientDeviceInfo(_httpContextAccessor.HttpContext?.Request?.Headers?.UserAgent);
        await _sysUserRep.AsUpdateable(user).UpdateColumns(u => new
        {
            u.LastLoginIp,
            u.LastLoginAddress,
            u.LastLoginTime,
            u.LastLoginDevice,
        }).ExecuteCommandAsync();

        return new LoginOutput
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    /// <summary>
    /// 获取登录账号 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取登录账号")]
    public virtual async Task<LoginUserOutput> GetUserInfo()
    {
        var user = await _sysUserRep.GetFirstAsync(u => u.Id == _userManager.UserId) ?? throw Oops.Oh(ErrorCodeEnum.D1011).StatusCode(401);
        // 获取机构
        var org = await _sysUserRep.ChangeRepository<SqlSugarRepository<SysOrg>>().GetFirstAsync(u => u.Id == user.OrgId);
        // 获取职位
        var pos = await _sysUserRep.ChangeRepository<SqlSugarRepository<SysPos>>().GetFirstAsync(u => u.Id == user.PosId);
        // 获取按钮集合
        var buttons = await _sysMenuService.GetOwnBtnPermList();
        // 获取角色集合
        var roleIds = await _sysUserRep.ChangeRepository<SqlSugarRepository<SysUserRole>>().AsQueryable()
            .Where(u => u.UserId == user.Id).Select(u => u.RoleId).ToListAsync();
        // 获取水印文字（若系统水印为空则全局为空）
        var watermarkText = await _sysConfigService.GetConfigValue<string>(ConfigConst.SysWebWatermark);
        if (!string.IsNullOrWhiteSpace(watermarkText))
            watermarkText += $"-{user.RealName}"; // $"-{user.RealName}-{_httpContextAccessor.HttpContext.GetRemoteIpAddressToIPv4(true)}-{DateTime.Now}";
        return new LoginUserOutput
        {
            Id = user.Id,
            Account = user.Account,
            RealName = user.RealName,
            Phone = user.Phone,
            IdCardNum = user.IdCardNum,
            Email = user.Email,
            AccountType = user.AccountType,
            Avatar = user.Avatar,
            Address = user.Address,
            Signature = user.Signature,
            OrgId = user.OrgId,
            OrgName = org?.Name,
            OrgType = org?.Type,
            PosName = pos?.Name,
            Buttons = buttons,
            RoleIds = roleIds,
            WatermarkText = watermarkText
        };
    }

    /// <summary>
    /// 获取刷新Token 🔖
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    [DisplayName("获取刷新Token")]
    public virtual string GetRefreshToken([FromQuery] string accessToken)
    {
        var refreshTokenExpire = _sysConfigService.GetRefreshTokenExpire().GetAwaiter().GetResult();
        return JWTEncryption.GenerateRefreshToken(accessToken, refreshTokenExpire);
    }

    /// <summary>
    /// 退出系统 🔖
    /// </summary>
    [DisplayName("退出系统")]
    public void Logout()
    {
        if (string.IsNullOrWhiteSpace(_userManager.Account))
            throw Oops.Oh(ErrorCodeEnum.D1011);

        _httpContextAccessor.HttpContext.SignoutToSwagger();
    }

    /// <summary>
    /// 获取验证码 🔖
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [SuppressMonitor]
    [DisplayName("获取验证码")]
    public dynamic GetCaptcha()
    {
        var codeId = YitIdHelper.NextId().ToString();
        var captcha = _captcha.Generate(codeId);
        var expirySeconds = App.GetOptions<CaptchaOptions>()?.ExpirySeconds ?? 60;
        return new { Id = codeId, Img = captcha.Base64, ExpirySeconds = expirySeconds };
    }

    /// <summary>
    /// Swagger登录检查 🔖
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("/api/swagger/checkUrl"), NonUnify]
    [ApiDescriptionSettings(Description = "Swagger登录检查", DisableInherite = true)]
    public int SwaggerCheckUrl()
    {
        return _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated ? 200 : 401;
    }

    /// <summary>
    /// Swagger登录提交 🔖
    /// </summary>
    /// <param name="auth"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("/api/swagger/submitUrl"), NonUnify]
    [ApiDescriptionSettings(Description = "Swagger登录提交", DisableInherite = true)]
    public async Task<int> SwaggerSubmitUrl([FromForm] SpecificationAuth auth)
    {
        try
        {
            _sysCacheService.Set($"{CacheConst.KeyConfig}{ConfigConst.SysCaptcha}", false);

            await Login(new LoginInput
            {
                Account = auth.UserName,
                Password = CryptogramUtil.SM2Encrypt(auth.Password),
                Host = SqlSugarConst.DefaultTenantHost
            });

            _sysCacheService.Remove($"{CacheConst.KeyConfig}{ConfigConst.SysCaptcha}");

            return 200;
        }
        catch (Exception)
        {
            return 401;
        }
    }
}