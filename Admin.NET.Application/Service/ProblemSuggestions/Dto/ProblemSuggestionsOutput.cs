﻿// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Application;

/// <summary>
/// 问题建议表输出参数
/// </summary>
public class ProblemSuggestionsOutput
{
    /// <summary>
    /// 主键Id
    /// </summary>
    public long Id { get; set; }    
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; }    
    
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }    
    
    /// <summary>
    /// 创建者Id
    /// </summary>
    public long? CreateUserId { get; set; }    
    
    /// <summary>
    /// 创建者姓名
    /// </summary>
    public string? CreateUserName { get; set; }    
    
    /// <summary>
    /// 修改者Id
    /// </summary>
    public long? UpdateUserId { get; set; }    
    
    /// <summary>
    /// 修改者姓名
    /// </summary>
    public string? UpdateUserName { get; set; }    
    
    /// <summary>
    /// 创建者部门Id
    /// </summary>
    public long? CreateOrgId { get; set; }    
    
    /// <summary>
    /// 创建者部门名称
    /// </summary>
    public string? CreateOrgName { get; set; }    
    
    /// <summary>
    /// 软删除
    /// </summary>
    public bool IsDelete { get; set; }    
    
    /// <summary>
    /// 问题id
    /// </summary>
    public long? ProbleId { get; set; }    
    
    /// <summary>
    /// 发表人
    /// </summary>
    public long? UserId { get; set; }    
    
    /// <summary>
    /// 发表人姓名
    /// </summary>
    public string? UserName { get; set; }    
    
    /// <summary>
    /// 建议人部门
    /// </summary>
    public long? UserDeptId { get; set; }    
    
    /// <summary>
    /// 建议人部门名称
    /// </summary>
    public string? UserDeptName { get; set; }    
    
    /// <summary>
    /// 发表内容
    /// </summary>
    public string? Content { get; set; }    
    
    /// <summary>
    /// 是否采纳
    /// </summary>
    public string? Status { get; set; }    
    
    /// <summary>
    /// 问题id
    /// </summary>
    public long? ProblemId { get; set; }    
    
    /// <summary>
    /// 发表人部门id
    /// </summary>
    public long? DeptId { get; set; }    
    
    /// <summary>
    /// 发表人部门名称
    /// </summary>
    public string? DeptName { get; set; }    
    
    /// <summary>
    /// 是否采纳
    /// </summary>
    public string? Floag { get; set; }    
    
    /// <summary>
    /// 发表时间
    /// </summary>
    public DateTime? PublishTime { get; set; }    
    
}

/// <summary>
/// 问题建议表数据导入模板实体
/// </summary>
public class ExportProblemSuggestionsOutput : ImportProblemSuggestionsInput
{
    [ImporterHeader(IsIgnore = true)]
    [ExporterHeader(IsIgnore = true)]
    public override string Error { get; set; }
}
