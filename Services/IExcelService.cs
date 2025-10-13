using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Highdmin.Services
{
    public interface IExcelService
    {
        Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName, Dictionary<string, string> columnMappings);
        Task<List<T>> ImportFromExcelAsync<T>(IFormFile file, Dictionary<string, string> columnMappings, Func<Dictionary<string, string>, T> mapFunction);
        byte[] GenerateTemplate(Dictionary<string, string> columnMappings, string sheetName, Dictionary<string, string[]>? validationValues = null, List<Dictionary<string, string>>? exampleData = null);
    }

    public class ExcelImportResult<T>
    {
        public List<T> Data { get; set; } = new();
        public List<string> Errors { get; set; } = new();
        public bool HasErrors => Errors.Any();
        public int TotalRows { get; set; }
        public int ProcessedRows => Data.Count;
    }

    public class ExcelColumn
    {
        public string PropertyName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string[]? ValidationValues { get; set; }
        public bool IsRequired { get; set; }
        public int MaxLength { get; set; }
        public Type DataType { get; set; } = typeof(string);
    }
}