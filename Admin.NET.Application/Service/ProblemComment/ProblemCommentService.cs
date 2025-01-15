// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Admin.NET.Application.Entity;
using Admin.NET.Core.Service;
using Microsoft.AspNetCore.Http;

namespace Admin.NET.Application;

/// <summary>
/// 问题评论服务 🧩
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Order = 100)]
public class ProblemCommentService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<ProblemComment> _problemCommentRep;
    private readonly ISqlSugarClient _sqlSugarClient;
    private readonly SqlSugarRepository<SysUser> _userRep;
    private readonly SqlSugarRepository<SysOrg> _orgRep;

    public ProblemCommentService(SqlSugarRepository<ProblemComment> problemCommentRep, ISqlSugarClient sqlSugarClient, SqlSugarRepository<SysUser> userRep, SqlSugarRepository<SysOrg> orgRep)
    {
        _problemCommentRep = problemCommentRep;
        _sqlSugarClient = sqlSugarClient;
        _userRep = userRep;
        _orgRep = orgRep;
    }

    /// <summary>
    /// 分页查询问题评论 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("分页查询问题评论")]
    [ApiDescriptionSettings(Name = "Page"), HttpPost]
    public async Task<SqlSugarPagedList<ProblemCommentOutput>> Page(PageProblemCommentInput input)
    {
        input.Keyword = input.Keyword?.Trim();
        var query = _problemCommentRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.Keyword), u => u.UserName.Contains(input.Keyword) || u.DeptName.Contains(input.Keyword) || u.Content.Contains(input.Keyword))
            .WhereIF(!string.IsNullOrWhiteSpace(input.UserName), u => u.UserName.Contains(input.UserName.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.DeptName), u => u.DeptName.Contains(input.DeptName.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Content), u => u.Content.Contains(input.Content.Trim()))
            .WhereIF(input.UserId != null, u => u.UserId == input.UserId)
            .WhereIF(input.DeptId != null, u => u.DeptId == input.DeptId)
            .WhereIF(input.CommentTimeRange?.Length == 2, u => u.CommentTime >= input.CommentTimeRange[0] && u.CommentTime <= input.CommentTimeRange[1])
            .Select<ProblemCommentOutput>();
		return await query.OrderBuilder(input).ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取问题评论详情 ℹ️
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取问题评论详情")]
    [ApiDescriptionSettings(Name = "Detail"), HttpGet]
    public async Task<ProblemComment> Detail([FromQuery] QueryByIdProblemCommentInput input)
    {
        return await _problemCommentRep.GetFirstAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// 增加问题评论 ➕
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("增加问题评论")]
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    public async Task<long> Add(AddProblemCommentInput input)
    {
        var entity = input.Adapt<ProblemComment>();
        var User = await _userRep.AsQueryable().ClearFilter().Where(x => x.Id == entity.UserId).FirstAsync();
        var UserDept = await _orgRep.AsQueryable().ClearFilter().Where(x => x.Id == User.OrgId).FirstAsync();
        entity.UserName = User == null ? "" : User.RealName;
        entity.DeptName = UserDept == null ? "" : UserDept.Name;
        entity.CommentTime = DateTime.Now;
        return await _problemCommentRep.InsertAsync(entity) ? entity.Id : 0;
    }

    /// <summary>
    /// 更新问题评论 ✏️
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("更新问题评论")]
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    public async Task Update(UpdateProblemCommentInput input)
    {
        var entity = input.Adapt<ProblemComment>();
        await _problemCommentRep.AsUpdateable(entity)
        .ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除问题评论 ❌
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("删除问题评论")]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    public async Task Delete(DeleteProblemCommentInput input)
    {
        var entity = await _problemCommentRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        await _problemCommentRep.FakeDeleteAsync(entity);   //假删除
        //await _problemCommentRep.DeleteAsync(entity);   //真删除
    }

    /// <summary>
    /// 批量删除问题评论 ❌
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("批量删除问题评论")]
    [ApiDescriptionSettings(Name = "BatchDelete"), HttpPost]
    public async Task<int> BatchDelete([Required(ErrorMessage = "主键列表不能为空")]List<DeleteProblemCommentInput> input)
    {
        var exp = Expressionable.Create<ProblemComment>();
        foreach (var row in input) exp = exp.Or(it => it.Id == row.Id);
        var list = await _problemCommentRep.AsQueryable().Where(exp.ToExpression()).ToListAsync();
        return await _problemCommentRep.FakeDeleteAsync(list);   //假删除
        //return await _problemCommentRep.DeleteAsync(list);   //真删除
    }
}
