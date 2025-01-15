// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Admin.NET.Application.Entity;
using Admin.NET.Core.Service;
using AngleSharp.Dom;
using Microsoft.AspNetCore.Http;

namespace Admin.NET.Application;

/// <summary>
/// 问题建议表服务 🧩
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Order = 100)]
public class ProblemSuggestionsService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<ProblemSuggestions> _problemSuggestionsRep;
    private readonly ISqlSugarClient _sqlSugarClient;
    private readonly SqlSugarRepository<SysUser> _userRep;
    private readonly SqlSugarRepository<SysOrg> _orgRep;
    public ProblemSuggestionsService(SqlSugarRepository<ProblemSuggestions> problemSuggestionsRep, ISqlSugarClient sqlSugarClient, SqlSugarRepository<SysUser> userRep, SqlSugarRepository<SysOrg> orgRep)
    {
        _problemSuggestionsRep = problemSuggestionsRep;
        _sqlSugarClient = sqlSugarClient;
        _userRep = userRep;
        _orgRep = orgRep;
    }

    /// <summary>
    /// 分页查询问题建议表 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("分页查询问题建议表")]
    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    public async Task<SqlSugarPagedList<ProblemSuggestionsOutput>> Page(PageProblemSuggestionsInput input)
    {
        input.Keyword = input.Keyword?.Trim();
        var query = _problemSuggestionsRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.Keyword), u => u.UserName.Contains(input.Keyword) || u.UserDeptName.Contains(input.Keyword) || u.Content.Contains(input.Keyword) || u.Status.Contains(input.Keyword) || u.DeptName.Contains(input.Keyword) || u.Floag.Contains(input.Keyword))
            .WhereIF(!string.IsNullOrWhiteSpace(input.UserName), u => u.UserName.Contains(input.UserName.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.UserDeptName), u => u.UserDeptName.Contains(input.UserDeptName.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Content), u => u.Content.Contains(input.Content.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Status), u => u.Status.Contains(input.Status.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.DeptName), u => u.DeptName.Contains(input.DeptName.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Floag), u => u.Floag.Contains(input.Floag.Trim()))
            .WhereIF(input.ProblemId != null, u => u.ProblemId == input.ProblemId)
            .Select<ProblemSuggestionsOutput>();
		return await query.OrderBuilder(input).ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取问题建议表详情 ℹ️
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取问题建议表详情")]
    [ApiDescriptionSettings(Name = "Detail"), HttpGet]
    public async Task<ProblemSuggestions> Detail([FromQuery] QueryByIdProblemSuggestionsInput input)
    {
        return await _problemSuggestionsRep.GetFirstAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// 增加问题建议表 ➕
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("增加问题建议表")]
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    public async Task<long> Add(AddProblemSuggestionsInput input)
    {
        var entity = input.Adapt<ProblemSuggestions>();
        var User = await _userRep.AsQueryable().ClearFilter().Where(x => x.Id == entity.UserId).FirstAsync();
        var UserDept = await _orgRep.AsQueryable().ClearFilter().Where(x => x.Id == User.OrgId).FirstAsync();
        entity.UserName = User == null ? "" : User.RealName;
        entity.DeptName = UserDept == null ? "" : UserDept.Name;
        entity.Floag = "";
        entity.PublishTime = DateTime.Now;
        return await _problemSuggestionsRep.InsertAsync(entity) ? entity.Id : 0;
    }

    /// <summary>
    /// 是否采纳建议 ✏️
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("是否采纳建议")]
    [ApiDescriptionSettings(Name = "Adopt"), HttpPost]
    public async Task Adopt(UpdateProblemSuggestionsInput input) 
    {
        var model = await _problemSuggestionsRep.AsQueryable().ClearFilter().Where(x => x.Id == input.Id).FirstAsync();
        if (model == null)
            throw Oops.Oh(ErrorCodeEnum.D1002);
        model.Status = input.Status;
        await _problemSuggestionsRep.AsUpdateable(model)
       .ExecuteCommandAsync();
    }

    /// <summary>
    /// 更新问题建议表 ✏️
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("更新问题建议表")]
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    public async Task Update(UpdateProblemSuggestionsInput input)
    {
        var entity = input.Adapt<ProblemSuggestions>();
        await _problemSuggestionsRep.AsUpdateable(entity)
        .ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除问题建议表 ❌
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("删除问题建议表")]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    public async Task Delete(DeleteProblemSuggestionsInput input)
    {
        var entity = await _problemSuggestionsRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        await _problemSuggestionsRep.FakeDeleteAsync(entity);   //假删除
        //await _problemSuggestionsRep.DeleteAsync(entity);   //真删除
    }
}
