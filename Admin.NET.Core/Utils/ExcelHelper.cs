// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using OfficeOpenXml;

namespace Admin.NET.Core;

public class ExcelHelper
{
    /// <summary>
    /// 数据导入
    /// </summary>
    /// <param name="file"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IActionResult ImportData<IN, T>(IFormFile file, Action<List<IN>, Action<StorageableResult<T>, List<IN>, List<T>>> action) where IN : BaseImportInput, new() where T : EntityBaseId, new()
    {
        try
        {
            var result = CommonUtil.ImportExcelDataAsync<IN>(file).Result ?? throw Oops.Oh("有效数据为空");

            var tasks = new List<Task>();
            action.Invoke(result, (storageable, pageItems, rows) =>
            {
                // 标记校验信息
                tasks.Add(Task.Run(() =>
                {
                    if (storageable.TotalList.Any())
                    {
                        for (int i = 0; i < rows.Count; i++) pageItems[i].Id = rows[i].Id;

                        for (int i = 0; i < storageable.TotalList.Count; i++)
                            pageItems[i].Error ??= storageable.TotalList[i].StorageMessage;
                    }
                }));
            });

            // 等待所有标记验证信息任务完成
            Task.WhenAll(tasks).GetAwaiter().GetResult();

            return ExportData(result);
        }
        catch (Exception ex)
        {
            App.HttpContext.Response.Headers["Content-Type"] = "application/json; charset=utf-8";
            throw Oops.Oh(new AdminResult<object>
            {
                Code = 500,
                Message = ex.Message,
                Result = null,
                Type = "error",
                Extras = UnifyContext.Take(),
                Time = DateTime.Now
            }.ToJson());
        }
    }

    /// <summary>
    /// 导出Xlsx数据
    /// </summary>
    /// <param name="list"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static IActionResult ExportData(dynamic list, string fileName = "导入记录")
    {
        var exporter = new ExcelExporter();
        var fs = new MemoryStream(exporter.ExportAsByteArray(list).GetAwaiter().GetResult());
        return new XlsxFileResult(stream: fs, fileDownloadName: $"{fileName}-{DateTime.Now:yyyy-MM-dd_HHmmss}");
    }

    /// <summary>
    /// 根据类型导出Xlsx模板
    /// </summary>
    /// <param name="list"></param>
    /// <param name="filename"></param>
    /// <param name="addListValidationFun"></param>
    /// <returns></returns>
    public static IActionResult ExportTemplate<T>(IEnumerable<T> list, string filename = "导入模板", Func<ExcelWorksheet, PropertyInfo, IEnumerable<string>> addListValidationFun = null)
    {
        using var package = new ExcelPackage((ExportData(list, filename) as XlsxFileResult)!.Stream);
        var worksheet = package.Workbook.Worksheets[0];

        foreach (var prop in typeof(T).GetProperties())
        {
            var propType = prop.PropertyType;

            var headerAttr = prop.GetCustomAttribute<ExporterHeaderAttribute>();
            var isNullableEnum = propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>) && Nullable.GetUnderlyingType(propType).IsEnum();
            if (isNullableEnum) propType = Nullable.GetUnderlyingType(propType);
            if (headerAttr == null) continue;

            // 获取列序号
            var columnIndex = 0;
            foreach (var item in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                if (++columnIndex > 0 && item.Text.Equals(headerAttr.DisplayName)) break;
            if (columnIndex <= 0) continue;

            // 优先从代理函数中获取下列列表，若为空且字段为枚举型，则填充枚举项为下列列表，若为字典字段，则填充字典值value列表为下列列表
            var dataList = addListValidationFun?.Invoke(worksheet, prop)?.ToList();
            if (dataList == null)
            {
                if (propType.IsEnum())
                {// 填充枚举项为下列列表
                    dataList = propType.EnumToList()?.Select(it => it.Describe).ToList();
                }
                else
                {// 获取字段上的字典特性
                    var dict = prop.GetCustomAttribute<DictAttribute>();
                    if (dict != null)
                    {// 填充字典值value为下列列表
                        dataList = App.GetService<SysDictTypeService>().GetDataList(new GetDataDictTypeInput
                            { Code = dict.DictTypeCode }).Result?.Select(x => x.Value).ToList();
                    }
                }
            }
            if (dataList != null) AddListValidation(columnIndex, dataList);
        }

        void AddListValidation(int columnIndex, List<string> dataList)
        {
            var validation = worksheet.DataValidations.AddListValidation(worksheet.Cells[2, columnIndex, 99999, columnIndex].Address);
            dataList.ForEach(e => validation!.Formula.Values.Add(e));
            validation.ShowErrorMessage = true;
            validation.ErrorTitle = "无效输入";
            validation.Error = "请从列表中选择一个有效的选项";
        }

        package.Save();
        package.Stream.Position = 0;
        return new XlsxFileResult(stream: package.Stream, fileDownloadName: $"{filename}-{DateTime.Now:yyyy-MM-dd_HHmmss}");
    }
}