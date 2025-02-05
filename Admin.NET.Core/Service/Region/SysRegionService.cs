// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using AngleSharp;
using AngleSharp.Html.Dom;

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统行政区域服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 310)]
public class SysRegionService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysRegion> _sysRegionRep;
    private readonly SysConfigService _sysConfigService;

    // Url地址-国家统计局行政区域2023年
    private readonly string _url = "http://www.stats.gov.cn/sj/tjbz/tjyqhdmhcxhfdm/2023/index.html";

    public SysRegionService(SqlSugarRepository<SysRegion> sysRegionRep, SysConfigService sysConfigService)
    {
        _sysRegionRep = sysRegionRep;
        _sysConfigService = sysConfigService;
    }

    /// <summary>
    /// 获取行政区域分页列表 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取行政区域分页列表")]
    public async Task<SqlSugarPagedList<SysRegion>> Page(PageRegionInput input)
    {
        return await _sysRegionRep.AsQueryable()
            .WhereIF(input.Pid > 0, u => u.Pid == input.Pid || u.Id == input.Pid)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Code), u => u.Code.Contains(input.Code))
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取行政区域列表 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取行政区域列表")]
    public async Task<List<SysRegion>> GetList([FromQuery] RegionInput input)
    {
        return await _sysRegionRep.GetListAsync(u => u.Pid == input.Id);
    }

    /// <summary>
    /// 增加行政区域 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("增加行政区域")]
    public async Task<long> AddRegion(AddRegionInput input)
    {
        input.Code = input.Code?.Trim() ?? "";
        if (input.Code.Length != 12 && input.Code.Length != 9 && input.Code.Length != 6) throw Oops.Oh(ErrorCodeEnum.R2003);

        if (input.Pid != 0)
        {
            var pRegion = await _sysRegionRep.GetFirstAsync(u => u.Id == input.Pid);
            pRegion ??= await _sysRegionRep.GetFirstAsync(u => u.Code == input.Pid.ToString());
            if (pRegion == null) throw Oops.Oh(ErrorCodeEnum.D2000);
            input.Pid = pRegion.Id;
        }

        var isExist = await _sysRegionRep.IsAnyAsync(u => u.Name == input.Name && u.Code == input.Code);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.R2002);

        var sysRegion = input.Adapt<SysRegion>();
        var newRegion = await _sysRegionRep.AsInsertable(sysRegion).ExecuteReturnEntityAsync();
        return newRegion.Id;
    }

    /// <summary>
    /// 更新行政区域 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("更新行政区域")]
    public async Task UpdateRegion(UpdateRegionInput input)
    {
        input.Code = input.Code?.Trim() ?? "";
        if (input.Code.Length != 12 && input.Code.Length != 9 && input.Code.Length != 6) throw Oops.Oh(ErrorCodeEnum.R2003);

        var sysRegion = await _sysRegionRep.GetFirstAsync(u => u.Id == input.Id);
        if (sysRegion == null) throw Oops.Oh(ErrorCodeEnum.D1002);

        if (sysRegion.Pid != input.Pid && input.Pid != 0)
        {
            var pRegion = await _sysRegionRep.GetFirstAsync(u => u.Id == input.Pid);
            pRegion ??= await _sysRegionRep.GetFirstAsync(u => u.Code == input.Pid.ToString());
            if (pRegion == null) throw Oops.Oh(ErrorCodeEnum.D2000);

            input.Pid = pRegion.Id;
            var regionTreeList = await _sysRegionRep.AsQueryable().ToChildListAsync(u => u.Pid, input.Id, true);
            var childIdList = regionTreeList.Select(u => u.Id).ToList();
            if (childIdList.Contains(input.Pid)) throw Oops.Oh(ErrorCodeEnum.R2004);
        }

        if (input.Id == input.Pid) throw Oops.Oh(ErrorCodeEnum.R2001);

        var isExist = await _sysRegionRep.IsAnyAsync(u => (u.Name == input.Name && u.Code == input.Code) && u.Id != sysRegion.Id);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.R2002);

        //// 父Id不能为自己的子节点
        //var regionTreeList = await _sysRegionRep.AsQueryable().ToChildListAsync(u => u.Pid, input.Id, true);
        //var childIdList = regionTreeList.Select(u => u.Id).ToList();
        //if (childIdList.Contains(input.Pid))
        //    throw Oops.Oh(ErrorCodeEnum.R2001);

        await _sysRegionRep.AsUpdateable(input.Adapt<SysRegion>()).IgnoreColumns(true).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除行政区域 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除行政区域")]
    public async Task DeleteRegion(DeleteRegionInput input)
    {
        var regionTreeList = await _sysRegionRep.AsQueryable().ToChildListAsync(u => u.Pid, input.Id, true);
        var regionIdList = regionTreeList.Select(u => u.Id).ToList();
        await _sysRegionRep.DeleteAsync(u => regionIdList.Contains(u.Id));
    }

    /// <summary>
    /// 同步行政区域 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("同步行政区域")]
    public async Task Sync()
    {
        var syncLevel = await _sysConfigService.GetConfigValue<int>(ConfigConst.SysRegionSyncLevel);
        if (syncLevel is < 1 or > 5) syncLevel = 3;//默认区县级
        var context = BrowsingContext.New(AngleSharp.Configuration.Default.WithDefaultLoader());
        var dom = await context.OpenAsync(_url);

        // 省级
        var itemList = dom.QuerySelectorAll("table.provincetable tr.provincetr td a");
        if (itemList.Length == 0) throw Oops.Oh(ErrorCodeEnum.R2005);

        await _sysRegionRep.DeleteAsync(u => u.Id > 0);

        foreach (var element in itemList)
        {
            var item = (IHtmlAnchorElement)element;
            var list = new List<SysRegion>();

            var region = new SysRegion
            {
                Id = YitIdHelper.NextId(),
                Pid = 0,
                Name = item.TextContent,
                Remark = item.Href,
                Level = 1,
            };
            list.Add(region);

            // 市级
            if (!string.IsNullOrEmpty(item.Href))
            {
                var dom1 = await context.OpenAsync(item.Href);
                var itemList1 = dom1.QuerySelectorAll("table.citytable tr.citytr td a");
                for (var i1 = 0; i1 < itemList1.Length; i1 += 2)
                {
                    var item1 = (IHtmlAnchorElement)itemList1[i1 + 1];
                    var region1 = new SysRegion
                    {
                        Id = YitIdHelper.NextId(),
                        Pid = region.Id,
                        Name = item1.TextContent,
                        Code = itemList1[i1].TextContent,
                        Remark = item1.Href,
                        Level = 2,
                    };
                    
                    // 若URL中查询的一级行政区域缺少Code则通过二级区域填充
                    if (list.Count == 1 && !string.IsNullOrEmpty(region1.Code))
                        region.Code = region1.Code.Substring(0, 2).PadRight(region1.Code.Length, '0');
                    
                    // 同步层级为“1-省级”退出
                    if (syncLevel < 2) break;

                    list.Add(region1);

                    // 区县级
                    if (string.IsNullOrEmpty(item1.Href) || syncLevel <= 2) continue;

                    var dom2 = await context.OpenAsync(item1.Href);
                    var itemList2 = dom2.QuerySelectorAll("table.countytable tr.countytr td a");
                    for (var i2 = 0; i2 < itemList2.Length; i2 += 2)
                    {
                        var item2 = (IHtmlAnchorElement)itemList2[i2 + 1];
                        var region2 = new SysRegion
                        {
                            Id = YitIdHelper.NextId(),
                            Pid = region1.Id,
                            Name = item2.TextContent,
                            Code = itemList2[i2].TextContent,
                            Remark = item2.Href,
                            Level = 3,
                        };
                        list.Add(region2);

                        // 街道级
                        if (string.IsNullOrEmpty(item2.Href) || syncLevel <= 3) continue;

                        var dom3 = await context.OpenAsync(item2.Href);
                        var itemList3 = dom3.QuerySelectorAll("table.towntable tr.towntr td a");
                        for (var i3 = 0; i3 < itemList3.Length; i3 += 2)
                        {
                            var item3 = (IHtmlAnchorElement)itemList3[i3 + 1];
                            var region3 = new SysRegion
                            {
                                Id = YitIdHelper.NextId(),
                                Pid = region2.Id,
                                Name = item3.TextContent,
                                Code = itemList3[i3].TextContent,
                                Remark = item3.Href,
                                Level = 4,
                            };
                            list.Add(region3);

                            // 村级
                            if (string.IsNullOrEmpty(item3.Href) || syncLevel <= 4) continue;

                            var dom4 = await context.OpenAsync(item3.Href);
                            var itemList4 = dom4.QuerySelectorAll("table.villagetable tr.villagetr td");
                            for (var i4 = 0; i4 < itemList4.Length; i4 += 3)
                            {
                                list.Add(new SysRegion
                                {
                                    Id = YitIdHelper.NextId(),
                                    Pid = region3.Id,
                                    Name = itemList4[i4 + 2].TextContent,
                                    Code = itemList4[i4].TextContent,
                                    CityCode = itemList4[i4 + 1].TextContent,
                                    Level = 5,
                                });
                            }
                        }
                    }
                }
            }

            //按省份同步快速写入提升同步效率，全部一次性写入容易出现从统计局获取数据失败
            await _sysRegionRep.Context.Fastest<SysRegion>().BulkCopyAsync(list);
        }
    }
}