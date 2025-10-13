using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Highdmin.Services
{
    public class ImportExportService : IImportExportService
    {
        public async Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, ExportConfiguration<T> config) where T : class
        {
            using var stream = new MemoryStream();
            using (var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                var sheets = workbookPart.Workbook.AppendChild(new Sheets());
                sheets.Append(new Sheet
                {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = config.SheetName
                });

                // Crear fila de encabezados
                var headerRow = new Row { RowIndex = 1 };
                var columnIndex = 1;
                foreach (var mapping in config.ColumnMappings)
                {
                    var cell = new Cell
                    {
                        CellReference = GetColumnName(columnIndex) + "1",
                        DataType = CellValues.String,
                        CellValue = new CellValue(mapping.Value)
                    };
                    headerRow.AppendChild(cell);
                    columnIndex++;
                }
                sheetData.AppendChild(headerRow);

                // Agregar datos
                var rowIndex = 2;
                foreach (var item in data)
                {
                    var dataRow = new Row { RowIndex = (uint)rowIndex };
                    columnIndex = 1;

                    Dictionary<string, object> rowData;
                    if (config.CustomMapping != null)
                    {
                        rowData = config.CustomMapping(item);
                    }
                    else
                    {
                        rowData = GetDefaultRowData(item, config.ColumnMappings);
                    }

                    foreach (var mapping in config.ColumnMappings)
                    {
                        var value = rowData.ContainsKey(mapping.Key) ? rowData[mapping.Key]?.ToString() ?? "" : "";
                        var cell = new Cell
                        {
                            CellReference = GetColumnName(columnIndex) + rowIndex,
                            DataType = CellValues.String,
                            CellValue = new CellValue(value)
                        };
                        dataRow.AppendChild(cell);
                        columnIndex++;
                    }
                    
                    sheetData.AppendChild(dataRow);
                    rowIndex++;
                }

                workbookPart.Workbook.Save();
            }

            return await Task.FromResult(stream.ToArray());
        }

        public async Task<ImportResult<T>> ImportFromExcelAsync<T>(IFormFile file, ImportConfiguration<T> config) where T : class, new()
        {
            var result = new ImportResult<T>();

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;

                using var document = SpreadsheetDocument.Open(stream, false);
                var workbookPart = document.WorkbookPart;
                var worksheetPart = workbookPart!.WorksheetParts.First();
                var worksheet = worksheetPart.Worksheet;
                var sheetData = worksheet.GetFirstChild<SheetData>();
                var rows = sheetData!.Descendants<Row>().ToList();

                result.TotalRows = rows.Count - 1; // Exclude header

                // Procesar filas (saltar encabezado)
                for (int i = 1; i < rows.Count; i++)
                {
                    var row = rows[i];
                    var cells = row.Descendants<Cell>().ToArray();
                    var rowData = new Dictionary<string, string>();
                    var rowNumber = i + 1;

                    // Extraer datos de la fila
                    var columnIndex = 0;
                    foreach (var mapping in config.ColumnMappings)
                    {
                        var cellValue = columnIndex < cells.Length 
                            ? GetCellValue(workbookPart, cells[columnIndex])?.Trim() ?? ""
                            : "";
                        rowData[mapping.Key] = cellValue;
                        columnIndex++;
                    }

                    // Verificar si la fila está vacía
                    if (config.SkipEmptyRows && rowData.Values.All(string.IsNullOrWhiteSpace))
                    {
                        continue;
                    }

                    try
                    {
                        // Mapear a objeto
                        var mappedObject = config.MapFunction(rowData);
                        if (mappedObject != null)
                        {
                            // Validar objeto
                            var validationErrors = ValidateObject(mappedObject, rowNumber);
                            
                            // Validación personalizada si existe
                            if (config.CustomValidation != null)
                            {
                                validationErrors.AddRange(config.CustomValidation(mappedObject, rowNumber));
                            }

                            if (validationErrors.Any())
                            {
                                result.Errors.AddRange(validationErrors);
                            }
                            else
                            {
                                result.Data.Add(mappedObject);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add($"Fila {rowNumber}: Error al procesar los datos - {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Error al procesar el archivo: {ex.Message}");
            }

            return result;
        }

        public byte[] GenerateImportTemplate<T>(ImportConfiguration<T> config) where T : class
        {
            using var stream = new MemoryStream();
            using (var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                var sheets = workbookPart.Workbook.AppendChild(new Sheets());
                sheets.Append(new Sheet
                {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = config.SheetName
                });

                // Crear fila de encabezados
                var headerRow = new Row { RowIndex = 1 };
                var columnIndex = 1;
                foreach (var mapping in config.ColumnMappings)
                {
                    var cell = new Cell
                    {
                        CellReference = GetColumnName(columnIndex) + "1",
                        DataType = CellValues.String,
                        CellValue = new CellValue(mapping.Value)
                    };
                    headerRow.AppendChild(cell);
                    columnIndex++;
                }
                sheetData.AppendChild(headerRow);

                // Agregar datos de ejemplo
                if (config.ExampleData != null && config.ExampleData.Any())
                {
                    var rowIndex = 2;
                    foreach (var example in config.ExampleData)
                    {
                        var exampleRow = new Row { RowIndex = (uint)rowIndex };
                        columnIndex = 1;

                        foreach (var mapping in config.ColumnMappings)
                        {
                            var value = example.ContainsKey(mapping.Key) ? example[mapping.Key] : "";
                            var cell = new Cell
                            {
                                CellReference = GetColumnName(columnIndex) + rowIndex,
                                DataType = CellValues.String,
                                CellValue = new CellValue(value)
                            };
                            exampleRow.AppendChild(cell);
                            columnIndex++;
                        }
                        
                        sheetData.AppendChild(exampleRow);
                        rowIndex++;
                    }
                }

                workbookPart.Workbook.Save();
            }

            return stream.ToArray();
        }

        private Dictionary<string, object> GetDefaultRowData<T>(T item, Dictionary<string, string> columnMappings) where T : class
        {
            var result = new Dictionary<string, object>();
            var properties = typeof(T).GetProperties();
            
            foreach (var mapping in columnMappings)
            {
                var property = properties.FirstOrDefault(p => p.Name == mapping.Key);
                result[mapping.Key] = property?.GetValue(item) ?? "";
            }
            
            return result;
        }

        private List<string> ValidateObject<T>(T obj, int rowNumber) where T : class
        {
            var errors = new List<string>();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(obj);
                var displayAttribute = property.GetCustomAttribute<DisplayAttribute>();
                var propertyName = displayAttribute?.Name ?? property.Name;

                // Validar Required
                var requiredAttribute = property.GetCustomAttribute<RequiredAttribute>();
                if (requiredAttribute != null && (value == null || string.IsNullOrWhiteSpace(value.ToString())))
                {
                    errors.Add($"Fila {rowNumber}: {propertyName} es obligatorio");
                }

                // Validar StringLength
                var stringLengthAttribute = property.GetCustomAttribute<StringLengthAttribute>();
                if (stringLengthAttribute != null && value != null)
                {
                    var stringValue = value.ToString();
                    if (!string.IsNullOrEmpty(stringValue) && stringValue.Length > stringLengthAttribute.MaximumLength)
                    {
                        errors.Add($"Fila {rowNumber}: {propertyName} no puede tener más de {stringLengthAttribute.MaximumLength} caracteres");
                    }
                }
            }

            return errors;
        }

        private string GetCellValue(WorkbookPart workbookPart, Cell cell)
        {
            if (cell?.CellValue == null) return string.Empty;

            var value = cell.CellValue.InnerText;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                var sharedStringPart = workbookPart.SharedStringTablePart;
                if (sharedStringPart?.SharedStringTable != null)
                {
                    value = sharedStringPart.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
                }
            }

            return value;
        }

        private string GetColumnName(int columnNumber)
        {
            string columnName = "";
            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }
            return columnName;
        }
    }
}