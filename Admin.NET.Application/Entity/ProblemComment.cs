﻿// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Admin.NET.Core;
namespace Admin.NET.Application.Entity;

/// <summary>
/// 问题评论
/// </summary>
[Tenant("1300000000001")]
[SugarTable(null, "问题评论")]
public class ProblemComment : EntityBaseData
{
    /// <summary>
    /// 评论人id
    /// </summary>
    [SugarColumn(ColumnName = "UserId", ColumnDescription = "评论人id")]
    public virtual long? UserId { get; set; }
    
    /// <summary>
    /// 评论人姓名
    /// </summary>
    [SugarColumn(ColumnName = "UserName", ColumnDescription = "评论人姓名", Length = 32)]
    public virtual string? UserName { get; set; }
    
    /// <summary>
    /// 评论人部门id
    /// </summary>
    [SugarColumn(ColumnName = "DeptId", ColumnDescription = "评论人部门id")]
    public virtual long? DeptId { get; set; }
    
    /// <summary>
    /// 评论人部门名称
    /// </summary>
    [SugarColumn(ColumnName = "DeptName", ColumnDescription = "评论人部门名称", Length = 32)]
    public virtual string? DeptName { get; set; }
    
    /// <summary>
    /// 评论内容
    /// </summary>
    [SugarColumn(ColumnName = "Content", ColumnDescription = "评论内容", Length = 500)]
    public virtual string? Content { get; set; }
    
    /// <summary>
    /// 评论时间
    /// </summary>
    [SugarColumn(ColumnName = "CommentTime", ColumnDescription = "评论时间")]
    public virtual DateTime? CommentTime { get; set; }

    /// <summary>
    /// 问题id
    /// </summary>
    [SugarColumn(ColumnName = "ProblemId", ColumnDescription = "问题id")]
    public virtual long? ProblemId { get; set; }

}
