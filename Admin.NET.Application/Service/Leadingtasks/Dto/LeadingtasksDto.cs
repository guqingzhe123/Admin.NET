﻿// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Application;

/// <summary>
/// 带班任务上报输出参数
/// </summary>
public class LeadingtasksDto
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
    /// 带班计划id
    /// </summary>
    public long? PlanId { get; set; }
    
    /// <summary>
    /// 上报人员id
    /// </summary>
    public long? UserId { get; set; }
    
    /// <summary>
    /// 上报人员姓名
    /// </summary>
    public string? UserName { get; set; }
    
    /// <summary>
    /// 上报人员部门id
    /// </summary>
    public long? DeptId { get; set; }
    
    /// <summary>
    /// 上报人员部门名称
    /// </summary>
    public string? DeptName { get; set; }
    
    /// <summary>
    /// 上报地点
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// 上报内容
    /// </summary>
    public string? Content { get; set; }
    
    /// <summary>
    /// 上报时间
    /// </summary>
    public DateTime? Time { get; set; }
    
    /// <summary>
    /// 任务描述
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// 状态
    /// </summary>
    public string Status { get; set; }
    
    /// <summary>
    /// 任务类型
    /// </summary>
    public string Type { get; set; }
    
}
