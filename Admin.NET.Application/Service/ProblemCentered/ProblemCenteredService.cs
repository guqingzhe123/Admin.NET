// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Admin.NET.Application.Entity;
using Admin.NET.Core.Service;
using Aliyun.OSS.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OnceMi.AspNetCore.OSS;
using Yitter.IdGenerator;

namespace Admin.NET.Application;

/// <summary>
/// 问题中心服务 🧩
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Order = 100)]
public class ProblemCenteredService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<Problemcentered> _problemCenteredRep;
    private readonly ISqlSugarClient _sqlSugarClient;
    private readonly OSSProviderOptions _OSSProviderOptions;
    private readonly UploadOptions _uploadOptions;
    private readonly IOSSService _OSSService;
    private readonly SqlSugarRepository<SysUser> _userRep;
    private readonly SqlSugarRepository<SysOrg> _orgRep;
    private readonly SqlSugarRepository<SysFile> _sysFileRep;
    private readonly SqlSugarRepository<ProblemSuggestions> _problemSuggestions;
    private readonly SqlSugarRepository<ProblemComment> _problemComment;
    private readonly string _imageType = ".jpeg.jpg.png.bmp.gif.tif";

    public ProblemCenteredService(SqlSugarRepository<Problemcentered> problemCenteredRep, ISqlSugarClient sqlSugarClient, IOptions<OSSProviderOptions> oSSProviderOptions,
        IOptions<UploadOptions> uploadOptions,
        IOSSServiceFactory ossServiceFactory, SqlSugarRepository<SysFile> sysFileRep, SqlSugarRepository<SysUser> userRep, SqlSugarRepository<SysOrg> orgRep, SqlSugarRepository<ProblemSuggestions> problemSuggestions, SqlSugarRepository<ProblemComment> problemComment)
    {
        _problemCenteredRep = problemCenteredRep;
        _sqlSugarClient = sqlSugarClient;
        _OSSProviderOptions = oSSProviderOptions.Value;
        _uploadOptions = uploadOptions.Value;
        if (_OSSProviderOptions.Enabled)
            _OSSService = ossServiceFactory.Create(Enum.GetName(_OSSProviderOptions.Provider));
        _sysFileRep = sysFileRep;
        _userRep = userRep;
        _orgRep = orgRep;
        _problemSuggestions = problemSuggestions;
        _problemComment = problemComment;
    }

    /// <summary>
    /// 分页查询问题中心 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("分页查询问题中心")]
    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    public async Task<SqlSugarPagedList<ProblemCenteredOutput>> Page(PageProblemCenteredInput input)
    {
        input.Keyword = input.Keyword?.Trim();
        var query = _problemCenteredRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.Keyword), u => u.PlanName.Contains(input.Keyword) || u.PlaceName.Contains(input.Keyword) || u.UserName.Contains(input.Keyword) || u.Source.Contains(input.Keyword) || u.UserDeptName.Contains(input.Keyword) || u.Status.Contains(input.Keyword) || u.HandleUserName.Contains(input.Keyword) || u.HandleDeptName.Contains(input.Keyword))
            .WhereIF(!string.IsNullOrWhiteSpace(input.PlanName), u => u.PlanName.Contains(input.PlanName.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.PlaceName), u => u.PlaceName.Contains(input.PlaceName.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.UserName), u => u.UserName.Contains(input.UserName.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Source), u => u.Source.Contains(input.Source.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.UserDeptName), u => u.UserDeptName.Contains(input.UserDeptName.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Status), u => u.Status.Contains(input.Status.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.HandleUserName), u => u.HandleUserName.Contains(input.HandleUserName.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.HandleDeptName), u => u.HandleDeptName.Contains(input.HandleDeptName.Trim()))
            .WhereIF(input.PlanId != null, u => u.PlanId == input.PlanId)
            .WhereIF(input.PlaceId != null, u => u.PlaceId == input.PlaceId)
            .WhereIF(input.UserId != null, u => u.UserId == input.UserId)
            .WhereIF(input.UserDeptId != null, u => u.UserDeptId == input.UserDeptId)
            .WhereIF(input.ReportTimeRange?.Length == 2, u => u.ReportTime >= input.ReportTimeRange[0] && u.ReportTime <= input.ReportTimeRange[1])
            .WhereIF(input.HandleUserId != null, u => u.HandleUserId == input.HandleUserId)
            .WhereIF(input.HandleDeptId != null, u => u.HandleDeptId == input.HandleDeptId)
            .WhereIF(input.HandleTimeRange?.Length == 2, u => u.HandleTime >= input.HandleTimeRange[0] && u.HandleTime <= input.HandleTimeRange[1])
            .Select<ProblemCenteredOutput>();
		return await query.OrderBuilder(input).ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取问题中心详情 ℹ️
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取问题中心详情")]
    [ApiDescriptionSettings(Name = "Detail"), HttpGet]
    public async Task<Problemcentered> Detail([FromQuery] QueryByIdProblemCenteredInput input)
    {
        var entity = await _problemCenteredRep.GetFirstAsync(u => u.Id == input.Id);
        entity.Comments = await _problemComment.AsQueryable().ClearFilter().Where(x => x.ProblemId == entity.Id).ToListAsync();
        entity.Suggestions = await _problemSuggestions.AsQueryable().ClearFilter().Where(x => x.ProblemId == entity.Id).ToListAsync();
        return entity;
    }

    /// <summary>
    /// 增加问题中心 ➕
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("增加问题中心")]
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    public async Task<long> Add(AddProblemCenteredInput input)
    {
        var entity = input.Adapt<Problemcentered>();
        var repUser = await _userRep.AsQueryable().ClearFilter().Where(x => x.Id == entity.UserId).FirstAsync();
        var repUserDept = await _orgRep.AsQueryable().ClearFilter().Where(x => x.Id == repUser.OrgId).FirstAsync();
        entity.UserName = repUser == null ? "" : repUser.RealName;
        entity.UserDeptName = repUserDept == null ? "" : repUserDept.Name;
        entity.ReportTime = DateTime.Now;
        entity.Status = "待派单";
        entity.GiveUpCount = 0;
        return await _problemCenteredRep.InsertAsync(entity) ? entity.Id : 0;
    }


    /// <summary>
    /// 派单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("派单")]
    [ApiDescriptionSettings(Name = "Dispatch"), HttpPost]
    public async Task Dispatch(UpdateProblemCenteredInput input) 
    {
        var entity = await _problemCenteredRep.AsQueryable().ClearFilter().Where(x => x.Id == input.Id).FirstAsync();
        if (entity == null)
            throw Oops.Oh(ErrorCodeEnum.D1002);
        var hadleUser = await _userRep.AsQueryable().ClearFilter().Where(x => x.Id == entity.HandleUserId).FirstAsync();
        var hadleDept = await _orgRep.AsQueryable().ClearFilter().Where(x => x.Id == hadleUser.OrgId).FirstAsync();
        entity.HandleUserName = hadleUser == null ? "" : hadleUser.RealName;
        entity.HandleDeptName = hadleDept == null ? "" : hadleDept.Name;
        entity.Status = "待接单";
        await _problemCenteredRep.AsUpdateable(entity)
       .ExecuteCommandAsync();
    }
    /// <summary>
    /// 接单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("接单")]
    [ApiDescriptionSettings(Name = "Receiving"), HttpPost]
    public async Task Receiving(UpdateProblemCenteredInput input) 
    {
        var entity = await _problemCenteredRep.AsQueryable().ClearFilter().Where(x => x.Id == input.Id).FirstAsync();
        if (entity == null)
            throw Oops.Oh(ErrorCodeEnum.D1002);
        entity.Status = "待处理";
        await _problemCenteredRep.AsUpdateable(entity)
      .ExecuteCommandAsync();
    }
    /// <summary>
    /// 问题处理
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("问题处理")]
    [ApiDescriptionSettings(Name = "ProblemHandle"), HttpPost]
    public async Task ProblemHandle(UpdateProblemCenteredInput input) 
    {
        var entity = await _problemCenteredRep.AsQueryable().ClearFilter().Where(x => x.Id == input.Id).FirstAsync();
        if (entity == null)
            throw Oops.Oh(ErrorCodeEnum.D1002);
        entity.HandleContent = input.HandleContent;
        entity.HandleMp3 = input.HandleMp3;
        entity.HandleVideo = input.HandleVideo;
        entity.HandleImg = input.HandleImg;
        entity.HandleTime = DateTime.Now;
        entity.Status = "已处理";
        await _problemCenteredRep.AsUpdateable(entity)
      .ExecuteCommandAsync();
    }


    #region 上传文件
    /// <summary>
    /// 上传文件 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("上传文件")]
    public async Task<SysFile> UploadFile([FromForm] UploadFileInput input)
    {
        if (input.File == null) throw Oops.Oh(ErrorCodeEnum.D8000);

        // 判断是否重复上传的文件
        var sizeKb = input.File.Length / 1024; // 大小KB
        var fileMd5 = string.Empty;
        if (_uploadOptions.EnableMd5)
        {
            await using (var fileStream = input.File.OpenReadStream())
            {
                fileMd5 = OssUtils.ComputeContentMd5(fileStream, fileStream.Length);
            }
            // Mysql8 中如果使用了 utf8mb4_general_ci 之外的编码会出错，尽量避免在条件里使用.ToString()
            // 因为 Squsugar 并不是把变量转换为字符串来构造SQL语句，而是构造了CAST(123 AS CHAR)这样的语句，这样这个返回值是utf8mb4_general_ci，所以容易出错。
            var sysFile = await _sysFileRep.GetFirstAsync(u => u.FileMd5 == fileMd5 && u.SizeKb == sizeKb);
            if (sysFile != null) return sysFile;
        }

        // 验证文件类型
       // if (!_uploadOptions.ContentType.Contains(input.File.ContentType)) throw Oops.Oh($"{ErrorCodeEnum.D8001}:{input.File.ContentType}");

        // 验证文件大小
        if (sizeKb > _uploadOptions.MaxSize) throw Oops.Oh($"{ErrorCodeEnum.D8002}，允许最大：{_uploadOptions.MaxSize}KB");

        // 获取文件后缀
        var suffix = Path.GetExtension(input.File.FileName).ToLower(); // 后缀
        if (string.IsNullOrWhiteSpace(suffix))
            suffix = string.Concat(".", input.File.ContentType.AsSpan(input.File.ContentType.LastIndexOf('/') + 1));
        if (!string.IsNullOrWhiteSpace(suffix))
        {
            //var contentTypeProvider = FS.GetFileExtensionContentTypeProvider();
            //suffix = contentTypeProvider.Mappings.FirstOrDefault(u => u.Value == file.ContentType).Key;
            // 修改 image/jpeg 类型返回的 .jpeg、jpe 后缀
            if (suffix == ".jpeg" || suffix == ".jpe")
                suffix = ".jpg";
        }
        if (string.IsNullOrWhiteSpace(suffix)) throw Oops.Oh(ErrorCodeEnum.D8003);

        // 防止客户端伪造文件类型
        if (!string.IsNullOrWhiteSpace(input.AllowSuffix) && !input.AllowSuffix.Contains(suffix)) throw Oops.Oh(ErrorCodeEnum.D8003);
        //if (!VerifyFileExtensionName.IsSameType(file.OpenReadStream(), suffix))
        //    throw Oops.Oh(ErrorCodeEnum.D8001);

        // 文件存储位置
        var path = string.IsNullOrWhiteSpace(input.SavePath) ? _uploadOptions.Path : input.SavePath;
        path = path.ParseToDateTimeForRep();

        var newFile = input.Adapt<SysFile>();
        newFile.Id = YitIdHelper.NextId();
        newFile.BucketName = _OSSProviderOptions.Enabled ? _OSSProviderOptions.Bucket : "Local"; // 阿里云对bucket名称有要求，1.只能包括小写字母，数字，短横线（-）2.必须以小写字母或者数字开头  3.长度必须在3-63字节之间
        newFile.FileName = Path.GetFileNameWithoutExtension(input.File.FileName);
        newFile.Suffix = suffix;
        newFile.SizeKb = sizeKb;
        newFile.FilePath = path;
        newFile.FileMd5 = fileMd5;

        var finalName = newFile.Id + suffix; // 文件最终名称
        if (_OSSProviderOptions.Enabled)
        {
            newFile.Provider = Enum.GetName(_OSSProviderOptions.Provider);
            var filePath = string.Concat(path, "/", finalName);
            await _OSSService.PutObjectAsync(newFile.BucketName, filePath, input.File.OpenReadStream());
            //  http://<你的bucket名字>.oss.aliyuncs.com/<你的object名字>
            //  生成外链地址 方便前端预览
            switch (_OSSProviderOptions.Provider)
            {
                case OSSProvider.Aliyun:
                    newFile.Url = $"{(_OSSProviderOptions.IsEnableHttps ? "https" : "http")}://{newFile.BucketName}.{_OSSProviderOptions.Endpoint}/{filePath}";
                    break;

                case OSSProvider.QCloud:
                    newFile.Url = $"{(_OSSProviderOptions.IsEnableHttps ? "https" : "http")}://{newFile.BucketName}-{_OSSProviderOptions.Endpoint}.cos.{_OSSProviderOptions.Region}.myqcloud.com/{filePath}";
                    break;

                case OSSProvider.Minio:
                    // 获取Minio文件的下载或者预览地址
                    // newFile.Url = await GetMinioPreviewFileUrl(newFile.BucketName, filePath);// 这种方法生成的Url是有7天有效期的，不能这样使用
                    // 需要在MinIO中的Buckets开通对 Anonymous 的readonly权限
                    var customHost = _OSSProviderOptions.CustomHost;
                    if (string.IsNullOrWhiteSpace(customHost))
                        customHost = _OSSProviderOptions.Endpoint;
                    newFile.Url = $"{(_OSSProviderOptions.IsEnableHttps ? "https" : "http")}://{customHost}/{newFile.BucketName}/{filePath}";
                    break;
            }
        }
        else if (App.Configuration["SSHProvider:Enabled"].ToBoolean())
        {
            var fullPath = string.Concat(path.StartsWith('/') ? path : "/" + path, "/", finalName);
            using SSHHelper helper = new(App.Configuration["SSHProvider:Host"],
                App.Configuration["SSHProvider:Port"].ToInt(), App.Configuration["SSHProvider:Username"], App.Configuration["SSHProvider:Password"]);
            helper.UploadFile(input.File.OpenReadStream(), fullPath);
        }
        else
        {
            newFile.Provider = ""; // 本地存储 Provider 显示为空
            var filePath = Path.Combine(App.WebHostEnvironment.WebRootPath, path);
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            var realFile = Path.Combine(filePath, finalName);
            await using (var stream = File.Create(realFile))
            {
                await input.File.CopyToAsync(stream);
            }

            newFile.Url = $"{newFile.FilePath}/{newFile.Id + newFile.Suffix}";
        }
        await _sysFileRep.AsInsertable(newFile).ExecuteCommandAsync();
        return newFile;
    }
    #endregion


}
