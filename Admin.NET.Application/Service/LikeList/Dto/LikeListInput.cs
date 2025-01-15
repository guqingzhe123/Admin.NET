﻿// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Admin.NET.Core;
using System.ComponentModel.DataAnnotations;

namespace Admin.NET.Application;

/// <summary>
/// 点赞表基础输入参数
/// </summary>
public class LikeListBaseInput
{
    /// <summary>
    /// 主键Id
    /// </summary>
    public virtual long? Id { get; set; }
    
    /// <summary>
    /// 问题id
    /// </summary>
    public virtual long? ProblemId { get; set; }
    
    /// <summary>
    /// 点赞id
    /// </summary>
    public virtual long? UserId { get; set; }
    
}

/// <summary>
/// 点赞表分页查询输入参数
/// </summary>
public class PageLikeListInput : BasePageInput
{
    /// <summary>
    /// 问题id
    /// </summary>
    public long? ProblemId { get; set; }
    
    /// <summary>
    /// 点赞id
    /// </summary>
    public long? UserId { get; set; }
    
    /// <summary>
    /// 选中主键列表
    /// </summary>
     public List<long> SelectKeyList { get; set; }
}

/// <summary>
/// 点赞表增加输入参数
/// </summary>
public class AddLikeListInput
{
    /// <summary>
    /// 问题id
    /// </summary>
    public long? ProblemId { get; set; }
    
    /// <summary>
    /// 点赞id
    /// </summary>
    public long? UserId { get; set; }
    
}

/// <summary>
/// 点赞表删除输入参数
/// </summary>
public class DeleteLikeListInput
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [Required(ErrorMessage = "主键Id不能为空")]
    public long? Id { get; set; }
    
}

/// <summary>
/// 点赞表更新输入参数
/// </summary>
public class UpdateLikeListInput
{
    /// <summary>
    /// 主键Id
    /// </summary>    
    [Required(ErrorMessage = "主键Id不能为空")]
    public long? Id { get; set; }
    
    /// <summary>
    /// 问题id
    /// </summary>    
    public long? ProblemId { get; set; }
    
    /// <summary>
    /// 点赞id
    /// </summary>    
    public long? UserId { get; set; }
    
}

/// <summary>
/// 点赞表主键查询输入参数
/// </summary>
public class QueryByIdLikeListInput : DeleteLikeListInput
{
}

/// <summary>
/// 点赞表数据导入实体
/// </summary>
[ExcelImporter(SheetIndex = 1, IsOnlyErrorRows = true)]
public class ImportLikeListInput : BaseImportInput
{
    /// <summary>
    /// 问题id
    /// </summary>
    [ImporterHeader(Name = "问题id")]
    [ExporterHeader("问题id", Format = "", Width = 25, IsBold = true)]
    public long? ProblemId { get; set; }
    
    /// <summary>
    /// 点赞id
    /// </summary>
    [ImporterHeader(Name = "点赞id")]
    [ExporterHeader("点赞id", Format = "", Width = 25, IsBold = true)]
    public long? UserId { get; set; }
    
}
