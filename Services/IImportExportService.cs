namespace Highdmin.Services
{
    public interface IImportExportService
    {
        Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, ExportConfiguration<T> config) where T : class;
        Task<ImportResult<T>> ImportFromExcelAsync<T>(IFormFile file, ImportConfiguration<T> config) where T : class, new();
        byte[] GenerateImportTemplate<T>(ImportConfiguration<T> config) where T : class;
    }

    public class ExportConfiguration<T> where T : class
    {
        public string SheetName { get; set; } = "Data";
        public string FileName { get; set; } = "Export";
        public Dictionary<string, string> ColumnMappings { get; set; } = new();
        public Func<T, Dictionary<string, object>>? CustomMapping { get; set; }
    }

    public class ImportConfiguration<T> where T : class
    {
        public string SheetName { get; set; } = "Data";
        public Dictionary<string, string> ColumnMappings { get; set; } = new();
        public Dictionary<string, string[]>? ValidationValues { get; set; }
        public List<Dictionary<string, string>>? ExampleData { get; set; }
        public Func<Dictionary<string, string>, T?> MapFunction { get; set; } = null!;
        public Func<T, int, List<string>>? CustomValidation { get; set; }
        public bool SkipEmptyRows { get; set; } = true;
    }

    public class ImportResult<T> where T : class
    {
        public List<T> Data { get; set; } = new();
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public bool HasErrors => Errors.Any();
        public bool HasWarnings => Warnings.Any();
        public int TotalRows { get; set; }
        public int ProcessedRows => Data.Count;
        public int SkippedRows => TotalRows - ProcessedRows;
    }
}