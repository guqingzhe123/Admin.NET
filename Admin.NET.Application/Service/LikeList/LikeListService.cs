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
/// 点赞表服务 🧩
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Order = 100)]
public class LikeListService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<LikeList> _likeListRep;
    private readonly SqlSugarRepository<Problemcentered> _problemCenteredRep;
    private readonly ISqlSugarClient _sqlSugarClient;
    private readonly SqlSugarRepository<SysUser> _userRep;
    public LikeListService(SqlSugarRepository<LikeList> likeListRep, ISqlSugarClient sqlSugarClient, SqlSugarRepository<Problemcentered> problemCenteredRep, SqlSugarRepository<SysUser> userRep)
    {
        _likeListRep = likeListRep;
        _sqlSugarClient = sqlSugarClient;
        _problemCenteredRep = problemCenteredRep;
        _userRep = userRep;
    }

    /// <summary>
    /// 点赞➕
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("点赞")]
    [ApiDescriptionSettings(Name = "GiveUp"), HttpPost]
    public async Task GiveUp(AddLikeListInput input) 
    {
        var entity = await _problemCenteredRep.AsQueryable().ClearFilter().Where(x => x.Id == input.ProblemId).FirstAsync();
        if(entity==null)
            throw Oops.Oh(ErrorCodeEnum.D1002);
        var count = await _likeListRep.AsQueryable().ClearFilter().Where(x => x.ProblemId == input.ProblemId && x.UserId==input.UserId).CountAsync();
        var user = await _userRep.AsQueryable().ClearFilter().Where(x => x.Id == input.UserId).FirstAsync();
        if (count==0)
        {
            entity.GiveUpCount += 1;
            await _problemCenteredRep.AsUpdateable(entity)
             .ExecuteCommandAsync();
            var likeList = input.Adapt<LikeList>();
            likeList.UserName = user.RealName;
            await _likeListRep.InsertAsync(likeList);
        }
    }
    

    /// <summary>
    /// 取消点赞
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("取消点赞")]
    [ApiDescriptionSettings(Name = "CancelLikes"), HttpPost]
    public async Task CancelLikes(AddLikeListInput input)
    {
        var entity = await _likeListRep.AsQueryable().ClearFilter().Where(x => x.ProblemId == input.ProblemId && x.UserId == input.UserId).FirstAsync();
        var problem = await _problemCenteredRep.AsQueryable().ClearFilter().Where(x => x.Id == entity.ProblemId).FirstAsync();
        problem.GiveUpCount -= 1;
        await _problemCenteredRep.AsUpdateable(problem)
            .ExecuteCommandAsync();
        await _likeListRep.DeleteAsync(entity);   //真删除
    }

    /// <summary>
    /// 查询用户点赞列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("查询用户点赞列表")]
    [ApiDescriptionSettings(Name = "GetLikes"), HttpPost]
    public async Task<List<LikeList>> GetLikes(AddLikeListInput input) 
    {
        var entity = await _likeListRep.AsQueryable().ClearFilter().Where(x => x.UserId == input.UserId).ToListAsync();
        return entity;
    }
    /// <summary>
    /// 查询问题点赞用户列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("查询问题点赞用户列表")]
    [ApiDescriptionSettings(Name = "GetLikesByProble"), HttpPost]
    public async Task<List<LikeList>> GetLikesByProble(AddLikeListInput input) 
    {
        var entity = await _likeListRep.AsQueryable().ClearFilter().Where(x => x.ProblemId == input.ProblemId).ToListAsync();
        return entity;
    }
}
