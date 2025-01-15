// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Admin.NET.Core;
using System.ComponentModel.DataAnnotations;

namespace Admin.NET.Application;

/// <summary>
/// 问题中心基础输入参数
/// </summary>
public class ProblemCenteredBaseInput
{
    /// <summary>
    /// 主键Id
    /// </summary>
    public virtual long? Id { get; set; }
    
    /// <summary>
    /// 计划id
    /// </summary>
    public virtual long? PlanId { get; set; }
    
    /// <summary>
    /// 计划名称
    /// </summary>
    public virtual string? PlanName { get; set; }
    
    /// <summary>
    /// 点位id
    /// </summary>
    public virtual long? PlaceId { get; set; }
    
    /// <summary>
    /// 点位名称
    /// </summary>
    public virtual string? PlaceName { get; set; }
    
    /// <summary>
    /// 上报人id
    /// </summary>
    public virtual long? UserId { get; set; }
    
    /// <summary>
    /// 上报人名称
    /// </summary>
    public virtual string? UserName { get; set; }
    
    /// <summary>
    /// 上报内容
    /// </summary>
    public virtual string? ReportContent { get; set; }
    
    /// <summary>
    /// 问题来源
    /// </summary>
    public virtual string? Source { get; set; }
    
    /// <summary>
    /// 上报人部门Id
    /// </summary>
    public virtual long? UserDeptId { get; set; }
    
    /// <summary>
    /// 上报人部门名称
    /// </summary>
    public virtual string? UserDeptName { get; set; }
    
    /// <summary>
    /// 问题图片
    /// </summary>
    public virtual string? ReportImg { get; set; }
    
    /// <summary>
    /// 问题视频
    /// </summary>
    public virtual string? ReportVideo { get; set; }
    
    /// <summary>
    /// 问题音频
    /// </summary>
    public virtual string? ReportMp3 { get; set; }
    
    /// <summary>
    /// 上报时间
    /// </summary>
    public virtual DateTime? ReportTime { get; set; }
    
    /// <summary>
    /// 状态
    /// </summary>
    public virtual string? Status { get; set; }
    
    /// <summary>
    /// 处理人id
    /// </summary>
    public virtual long? HandleUserId { get; set; }
    
    /// <summary>
    /// 处理人名称
    /// </summary>
    public virtual string? HandleUserName { get; set; }
    
    /// <summary>
    /// 处理人部门id
    /// </summary>
    public virtual long? HandleDeptId { get; set; }
    
    /// <summary>
    /// 处理人部门
    /// </summary>
    public virtual string? HandleDeptName { get; set; }
    
    /// <summary>
    /// 处理结果
    /// </summary>
    public virtual string? HandleContent { get; set; }
    
    /// <summary>
    /// 处理图片
    /// </summary>
    public virtual string? HandleImg { get; set; }
    
    /// <summary>
    /// 处理视频
    /// </summary>
    public virtual string? HandleVideo { get; set; }
    
    /// <summary>
    /// 处理音频
    /// </summary>
    public virtual string? HandleMp3 { get; set; }
    
    /// <summary>
    /// 处理时间
    /// </summary>
    public virtual DateTime? HandleTime { get; set; }
    
}

/// <summary>
/// 问题中心分页查询输入参数
/// </summary>
public class PageProblemCenteredInput : BasePageInput
{
    /// <summary>
    /// 计划id
    /// </summary>
    public long? PlanId { get; set; }
    
    /// <summary>
    /// 计划名称
    /// </summary>
    public string? PlanName { get; set; }
    
    /// <summary>
    /// 点位id
    /// </summary>
    public long? PlaceId { get; set; }
    
    /// <summary>
    /// 点位名称
    /// </summary>
    public string? PlaceName { get; set; }
    
    /// <summary>
    /// 上报人id
    /// </summary>
    public long? UserId { get; set; }
    
    /// <summary>
    /// 上报人名称
    /// </summary>
    public string? UserName { get; set; }
    
    /// <summary>
    /// 问题来源
    /// </summary>
    public string? Source { get; set; }
    
    /// <summary>
    /// 上报人部门Id
    /// </summary>
    public long? UserDeptId { get; set; }
    
    /// <summary>
    /// 上报人部门名称
    /// </summary>
    public string? UserDeptName { get; set; }
    
    /// <summary>
    /// 上报时间范围
    /// </summary>
     public DateTime?[] ReportTimeRange { get; set; }
    
    /// <summary>
    /// 状态
    /// </summary>
    public string? Status { get; set; }
    
    /// <summary>
    /// 处理人id
    /// </summary>
    public long? HandleUserId { get; set; }
    
    /// <summary>
    /// 处理人名称
    /// </summary>
    public string? HandleUserName { get; set; }
    
    /// <summary>
    /// 处理人部门id
    /// </summary>
    public long? HandleDeptId { get; set; }
    
    /// <summary>
    /// 处理人部门
    /// </summary>
    public string? HandleDeptName { get; set; }
    
    /// <summary>
    /// 处理时间范围
    /// </summary>
     public DateTime?[] HandleTimeRange { get; set; }
    
    /// <summary>
    /// 选中主键列表
    /// </summary>
     public List<long> SelectKeyList { get; set; }
}

/// <summary>
/// 问题中心增加输入参数
/// </summary>
public class AddProblemCenteredInput
{
    /// <summary>
    /// 计划id
    /// </summary>
    public long? PlanId { get; set; }
    
    /// <summary>
    /// 计划名称
    /// </summary>
    [MaxLength(32, ErrorMessage = "计划名称字符长度不能超过32")]
    public string? PlanName { get; set; }
    
    /// <summary>
    /// 点位id
    /// </summary>
    public long? PlaceId { get; set; }
    
    /// <summary>
    /// 点位名称
    /// </summary>
    [MaxLength(32, ErrorMessage = "点位名称字符长度不能超过32")]
    public string? PlaceName { get; set; }
    
    /// <summary>
    /// 上报人id
    /// </summary>
    public long? UserId { get; set; }
    
    /// <summary>
    /// 上报人名称
    /// </summary>
    [MaxLength(32, ErrorMessage = "上报人名称字符长度不能超过32")]
    public string? UserName { get; set; }
    
    /// <summary>
    /// 上报内容
    /// </summary>
    [MaxLength(500, ErrorMessage = "上报内容字符长度不能超过500")]
    public string? ReportContent { get; set; }
    
    /// <summary>
    /// 问题来源
    /// </summary>
    [MaxLength(32, ErrorMessage = "问题来源字符长度不能超过32")]
    public string? Source { get; set; }
    
    /// <summary>
    /// 上报人部门Id
    /// </summary>
    public long? UserDeptId { get; set; }
    
    /// <summary>
    /// 上报人部门名称
    /// </summary>
    [MaxLength(32, ErrorMessage = "上报人部门名称字符长度不能超过32")]
    public string? UserDeptName { get; set; }
    
    /// <summary>
    /// 问题图片
    /// </summary>
    [MaxLength(500, ErrorMessage = "问题图片字符长度不能超过500")]
    public string? ReportImg { get; set; }
    
    /// <summary>
    /// 问题视频
    /// </summary>
    [MaxLength(500, ErrorMessage = "问题视频字符长度不能超过500")]
    public string? ReportVideo { get; set; }
    
    /// <summary>
    /// 问题音频
    /// </summary>
    [MaxLength(500, ErrorMessage = "问题音频字符长度不能超过500")]
    public string? ReportMp3 { get; set; }
    
    /// <summary>
    /// 上报时间
    /// </summary>
    public DateTime? ReportTime { get; set; }
    
    /// <summary>
    /// 状态
    /// </summary>
    [MaxLength(32, ErrorMessage = "状态字符长度不能超过32")]
    public string? Status { get; set; }
    
    /// <summary>
    /// 处理人id
    /// </summary>
    public long? HandleUserId { get; set; }
    
    /// <summary>
    /// 处理人名称
    /// </summary>
    [MaxLength(32, ErrorMessage = "处理人名称字符长度不能超过32")]
    public string? HandleUserName { get; set; }
    
    /// <summary>
    /// 处理人部门id
    /// </summary>
    public long? HandleDeptId { get; set; }
    
    /// <summary>
    /// 处理人部门
    /// </summary>
    [MaxLength(32, ErrorMessage = "处理人部门字符长度不能超过32")]
    public string? HandleDeptName { get; set; }
    
    /// <summary>
    /// 处理结果
    /// </summary>
    [MaxLength(500, ErrorMessage = "处理结果字符长度不能超过500")]
    public string? HandleContent { get; set; }
    
    /// <summary>
    /// 处理图片
    /// </summary>
    [MaxLength(500, ErrorMessage = "处理图片字符长度不能超过500")]
    public string? HandleImg { get; set; }
    
    /// <summary>
    /// 处理视频
    /// </summary>
    [MaxLength(500, ErrorMessage = "处理视频字符长度不能超过500")]
    public string? HandleVideo { get; set; }
    
    /// <summary>
    /// 处理音频
    /// </summary>
    [MaxLength(500, ErrorMessage = "处理音频字符长度不能超过500")]
    public string? HandleMp3 { get; set; }
    
    /// <summary>
    /// 处理时间
    /// </summary>
    public DateTime? HandleTime { get; set; }
    
}

/// <summary>
/// 问题中心删除输入参数
/// </summary>
public class DeleteProblemCenteredInput
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [Required(ErrorMessage = "主键Id不能为空")]
    public long? Id { get; set; }
    
}

/// <summary>
/// 问题中心更新输入参数
/// </summary>
public class UpdateProblemCenteredInput
{
    /// <summary>
    /// 主键Id
    /// </summary>    
    [Required(ErrorMessage = "主键Id不能为空")]
    public long? Id { get; set; }
    
    /// <summary>
    /// 计划id
    /// </summary>    
    public long? PlanId { get; set; }
    
    /// <summary>
    /// 计划名称
    /// </summary>    
    [MaxLength(32, ErrorMessage = "计划名称字符长度不能超过32")]
    public string? PlanName { get; set; }
    
    /// <summary>
    /// 点位id
    /// </summary>    
    public long? PlaceId { get; set; }
    
    /// <summary>
    /// 点位名称
    /// </summary>    
    [MaxLength(32, ErrorMessage = "点位名称字符长度不能超过32")]
    public string? PlaceName { get; set; }
    
    /// <summary>
    /// 上报人id
    /// </summary>    
    public long? UserId { get; set; }
    
    /// <summary>
    /// 上报人名称
    /// </summary>    
    [MaxLength(32, ErrorMessage = "上报人名称字符长度不能超过32")]
    public string? UserName { get; set; }
    
    /// <summary>
    /// 上报内容
    /// </summary>    
    [MaxLength(500, ErrorMessage = "上报内容字符长度不能超过500")]
    public string? ReportContent { get; set; }
    
    /// <summary>
    /// 问题来源
    /// </summary>    
    [MaxLength(32, ErrorMessage = "问题来源字符长度不能超过32")]
    public string? Source { get; set; }
    
    /// <summary>
    /// 上报人部门Id
    /// </summary>    
    public long? UserDeptId { get; set; }
    
    /// <summary>
    /// 上报人部门名称
    /// </summary>    
    [MaxLength(32, ErrorMessage = "上报人部门名称字符长度不能超过32")]
    public string? UserDeptName { get; set; }
    
    /// <summary>
    /// 问题图片
    /// </summary>    
    [MaxLength(500, ErrorMessage = "问题图片字符长度不能超过500")]
    public string? ReportImg { get; set; }
    
    /// <summary>
    /// 问题视频
    /// </summary>    
    [MaxLength(500, ErrorMessage = "问题视频字符长度不能超过500")]
    public string? ReportVideo { get; set; }
    
    /// <summary>
    /// 问题音频
    /// </summary>    
    [MaxLength(500, ErrorMessage = "问题音频字符长度不能超过500")]
    public string? ReportMp3 { get; set; }
    
    /// <summary>
    /// 上报时间
    /// </summary>    
    public DateTime? ReportTime { get; set; }
    
    /// <summary>
    /// 状态
    /// </summary>    
    [MaxLength(32, ErrorMessage = "状态字符长度不能超过32")]
    public string? Status { get; set; }
    
    /// <summary>
    /// 处理人id
    /// </summary>    
    public long? HandleUserId { get; set; }
    
    /// <summary>
    /// 处理人名称
    /// </summary>    
    [MaxLength(32, ErrorMessage = "处理人名称字符长度不能超过32")]
    public string? HandleUserName { get; set; }
    
    /// <summary>
    /// 处理人部门id
    /// </summary>    
    public long? HandleDeptId { get; set; }
    
    /// <summary>
    /// 处理人部门
    /// </summary>    
    [MaxLength(32, ErrorMessage = "处理人部门字符长度不能超过32")]
    public string? HandleDeptName { get; set; }
    
    /// <summary>
    /// 处理结果
    /// </summary>    
    [MaxLength(500, ErrorMessage = "处理结果字符长度不能超过500")]
    public string? HandleContent { get; set; }
    
    /// <summary>
    /// 处理图片
    /// </summary>    
    [MaxLength(500, ErrorMessage = "处理图片字符长度不能超过500")]
    public string? HandleImg { get; set; }
    
    /// <summary>
    /// 处理视频
    /// </summary>    
    [MaxLength(500, ErrorMessage = "处理视频字符长度不能超过500")]
    public string? HandleVideo { get; set; }
    
    /// <summary>
    /// 处理音频
    /// </summary>    
    [MaxLength(500, ErrorMessage = "处理音频字符长度不能超过500")]
    public string? HandleMp3 { get; set; }
    
    /// <summary>
    /// 处理时间
    /// </summary>    
    public DateTime? HandleTime { get; set; }
    
}

/// <summary>
/// 问题中心主键查询输入参数
/// </summary>
public class QueryByIdProblemCenteredInput : DeleteProblemCenteredInput
{
}

/// <summary>
/// 问题中心数据导入实体
/// </summary>
[ExcelImporter(SheetIndex = 1, IsOnlyErrorRows = true)]
public class ImportProblemCenteredInput : BaseImportInput
{
    /// <summary>
    /// 计划id
    /// </summary>
    [ImporterHeader(Name = "计划id")]
    [ExporterHeader("计划id", Format = "", Width = 25, IsBold = true)]
    public long? PlanId { get; set; }
    
    /// <summary>
    /// 计划名称
    /// </summary>
    [ImporterHeader(Name = "计划名称")]
    [ExporterHeader("计划名称", Format = "", Width = 25, IsBold = true)]
    public string? PlanName { get; set; }
    
    /// <summary>
    /// 点位id
    /// </summary>
    [ImporterHeader(Name = "点位id")]
    [ExporterHeader("点位id", Format = "", Width = 25, IsBold = true)]
    public long? PlaceId { get; set; }
    
    /// <summary>
    /// 点位名称
    /// </summary>
    [ImporterHeader(Name = "点位名称")]
    [ExporterHeader("点位名称", Format = "", Width = 25, IsBold = true)]
    public string? PlaceName { get; set; }
    
    /// <summary>
    /// 上报人id
    /// </summary>
    [ImporterHeader(Name = "上报人id")]
    [ExporterHeader("上报人id", Format = "", Width = 25, IsBold = true)]
    public long? UserId { get; set; }
    
    /// <summary>
    /// 上报人名称
    /// </summary>
    [ImporterHeader(Name = "上报人名称")]
    [ExporterHeader("上报人名称", Format = "", Width = 25, IsBold = true)]
    public string? UserName { get; set; }
    
    /// <summary>
    /// 上报内容
    /// </summary>
    [ImporterHeader(Name = "上报内容")]
    [ExporterHeader("上报内容", Format = "", Width = 25, IsBold = true)]
    public string? ReportContent { get; set; }
    
    /// <summary>
    /// 问题来源
    /// </summary>
    [ImporterHeader(Name = "问题来源")]
    [ExporterHeader("问题来源", Format = "", Width = 25, IsBold = true)]
    public string? Source { get; set; }
    
    /// <summary>
    /// 上报人部门Id
    /// </summary>
    [ImporterHeader(Name = "上报人部门Id")]
    [ExporterHeader("上报人部门Id", Format = "", Width = 25, IsBold = true)]
    public long? UserDeptId { get; set; }
    
    /// <summary>
    /// 上报人部门名称
    /// </summary>
    [ImporterHeader(Name = "上报人部门名称")]
    [ExporterHeader("上报人部门名称", Format = "", Width = 25, IsBold = true)]
    public string? UserDeptName { get; set; }
    
    /// <summary>
    /// 问题图片
    /// </summary>
    [ImporterHeader(Name = "问题图片")]
    [ExporterHeader("问题图片", Format = "", Width = 25, IsBold = true)]
    public string? ReportImg { get; set; }
    
    /// <summary>
    /// 问题视频
    /// </summary>
    [ImporterHeader(Name = "问题视频")]
    [ExporterHeader("问题视频", Format = "", Width = 25, IsBold = true)]
    public string? ReportVideo { get; set; }
    
    /// <summary>
    /// 问题音频
    /// </summary>
    [ImporterHeader(Name = "问题音频")]
    [ExporterHeader("问题音频", Format = "", Width = 25, IsBold = true)]
    public string? ReportMp3 { get; set; }
    
    /// <summary>
    /// 上报时间
    /// </summary>
    [ImporterHeader(Name = "上报时间")]
    [ExporterHeader("上报时间", Format = "", Width = 25, IsBold = true)]
    public DateTime? ReportTime { get; set; }
    
    /// <summary>
    /// 状态
    /// </summary>
    [ImporterHeader(Name = "状态")]
    [ExporterHeader("状态", Format = "", Width = 25, IsBold = true)]
    public string? Status { get; set; }
    
    /// <summary>
    /// 处理人id
    /// </summary>
    [ImporterHeader(Name = "处理人id")]
    [ExporterHeader("处理人id", Format = "", Width = 25, IsBold = true)]
    public long? HandleUserId { get; set; }
    
    /// <summary>
    /// 处理人名称
    /// </summary>
    [ImporterHeader(Name = "处理人名称")]
    [ExporterHeader("处理人名称", Format = "", Width = 25, IsBold = true)]
    public string? HandleUserName { get; set; }
    
    /// <summary>
    /// 处理人部门id
    /// </summary>
    [ImporterHeader(Name = "处理人部门id")]
    [ExporterHeader("处理人部门id", Format = "", Width = 25, IsBold = true)]
    public long? HandleDeptId { get; set; }
    
    /// <summary>
    /// 处理人部门
    /// </summary>
    [ImporterHeader(Name = "处理人部门")]
    [ExporterHeader("处理人部门", Format = "", Width = 25, IsBold = true)]
    public string? HandleDeptName { get; set; }
    
    /// <summary>
    /// 处理结果
    /// </summary>
    [ImporterHeader(Name = "处理结果")]
    [ExporterHeader("处理结果", Format = "", Width = 25, IsBold = true)]
    public string? HandleContent { get; set; }
    
    /// <summary>
    /// 处理图片
    /// </summary>
    [ImporterHeader(Name = "处理图片")]
    [ExporterHeader("处理图片", Format = "", Width = 25, IsBold = true)]
    public string? HandleImg { get; set; }
    
    /// <summary>
    /// 处理视频
    /// </summary>
    [ImporterHeader(Name = "处理视频")]
    [ExporterHeader("处理视频", Format = "", Width = 25, IsBold = true)]
    public string? HandleVideo { get; set; }
    
    /// <summary>
    /// 处理音频
    /// </summary>
    [ImporterHeader(Name = "处理音频")]
    [ExporterHeader("处理音频", Format = "", Width = 25, IsBold = true)]
    public string? HandleMp3 { get; set; }
    
    /// <summary>
    /// 处理时间
    /// </summary>
    [ImporterHeader(Name = "处理时间")]
    [ExporterHeader("处理时间", Format = "", Width = 25, IsBold = true)]
    public DateTime? HandleTime { get; set; }
    
}
