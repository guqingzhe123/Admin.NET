// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 应用基础输入参数
/// </summary>
public class SysAppBaseInput
{
    /// <summary>
    /// 主键Id
    /// </summary>
    public virtual long? Id { get; set; }
    
    /// <summary>
    /// 名称
    /// </summary>
    [Required(ErrorMessage = "名称不能为空")]
    public virtual string Name { get; set; }
    
    /// <summary>
    /// 标题
    /// </summary>
    [Required(ErrorMessage = "标题不能为空")]
    public virtual string Title { get; set; }
    
    /// <summary>
    /// 副标题
    /// </summary>
    [Required(ErrorMessage = "副标题不能为空")]
    public virtual string ViceTitle { get; set; }
    
    /// <summary>
    /// 副描述
    /// </summary>
    public virtual string? ViceDesc { get; set; }
    
    /// <summary>
    /// 水印
    /// </summary>
    public virtual string? Watermark { get; set; }
    
    /// <summary>
    /// 版权信息
    /// </summary>
    public virtual string? Copyright { get; set; }
    
    /// <summary>
    /// ICP备案号
    /// </summary>
    public virtual string? Icp { get; set; }
    
    /// <summary>
    /// 排序
    /// </summary>
    [Required(ErrorMessage = "排序不能为空")]
    public virtual int? OrderNo { get; set; }
    
    /// <summary>
    /// 备注
    /// </summary>
    public virtual string? Remark { get; set; }
    
    /// <summary>
    /// 图标
    /// </summary>
    public virtual string? Logo { get; set; }
}

/// <summary>
/// 应用增加输入参数
/// </summary>
public class AddSysAppInput
{
    /// <summary>
    /// 图标
    /// </summary>
    [Required(ErrorMessage = "Logo不能为空")]
    public string Logo { get; set; }
    
    /// <summary>
    /// 名称
    /// </summary>
    [Required(ErrorMessage = "名称不能为空")]
    public string Name { get; set; }
    
    /// <summary>
    /// 标题
    /// </summary>
    [Required(ErrorMessage = "标题不能为空")]
    public string Title { get; set; }
    
    /// <summary>
    /// 副标题
    /// </summary>
    [Required(ErrorMessage = "副标题不能为空")]
    public string ViceTitle { get; set; }
    
    /// <summary>
    /// 副描述
    /// </summary>
    [Required(ErrorMessage = "副描述不能为空")]
    public string ViceDesc { get; set; }
    
    /// <summary>
    /// 水印
    /// </summary>
    [Required(ErrorMessage = "水印不能为空")]
    public string Watermark { get; set; }
    
    /// <summary>
    /// 版权信息
    /// </summary>
    [Required(ErrorMessage = "版权信息不能为空")]
    public string Copyright { get; set; }
    
    /// <summary>
    /// ICP备案号
    /// </summary>
    [Required(ErrorMessage = "备案号不能为空")]
    public string Icp { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Required(ErrorMessage = "排序不能为空")]
    public int OrderNo { get; set; } = 100;
    
    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(256, ErrorMessage = "备注字符长度不能超过256")]
    public string Remark { get; set; }
}

/// <summary>
/// 应用更新输入参数
/// </summary>
public class UpdateSysAppInput : AddSysAppInput
{
    /// <summary>
    /// 主键Id
    /// </summary>    
    [Required(ErrorMessage = "主键Id不能为空")]
    public long Id { get; set; }
}

/// <summary>
/// 授权应用菜单
/// </summary>
public class UpdateAppMenuInput : BaseIdInput
{
    /// <summary>
    /// 菜单Id集合
    /// </summary>
    public List<long> MenuIdList { get; set; }
}

/// <summary>
/// 租户iId
/// </summary>
public class ChangeAppInput : BaseIdInput
{
    /// <summary>
    /// 租户Id
    /// </summary>
    [Required(ErrorMessage = "租户不能为空")]
    public long TenantId { get; set; }
}