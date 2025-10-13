using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace Highdmin.Services
{
    public class ExcelService : IExcelService
    {
        public async Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName, Dictionary<string, string> columnMappings)
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
                    Name = sheetName
                });

                // Crear fila de encabezados
                var headerRow = new Row { RowIndex = 1 };
                var columnIndex = 1;
                foreach (var mapping in columnMappings)
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
                var properties = typeof(T).GetProperties();
                
                foreach (var item in data)
                {
                    var dataRow = new Row { RowIndex = (uint)rowIndex };
                    columnIndex = 1;

                    foreach (var mapping in columnMappings)
                    {
                        var property = properties.FirstOrDefault(p => p.Name == mapping.Key);
                        var value = property?.GetValue(item)?.ToString() ?? "";

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

            return stream.ToArray();
        }

        public async Task<List<T>> ImportFromExcelAsync<T>(IFormFile file, Dictionary<string, string> columnMappings, Func<Dictionary<string, string>, T> mapFunction)
        {
            var result = new List<T>();

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using var document = SpreadsheetDocument.Open(stream, false);
            var workbookPart = document.WorkbookPart;
            var worksheetPart = workbookPart!.WorksheetParts.First();
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();
            var rows = sheetData!.Descendants<Row>().ToList();

            // Saltar la fila de encabezados
            for (int i = 1; i < rows.Count; i++)
            {
                var row = rows[i];
                var cells = row.Descendants<Cell>().ToArray();
                var rowData = new Dictionary<string, string>();

                var columnIndex = 0;
                foreach (var mapping in columnMappings)
                {
                    var cellValue = columnIndex < cells.Length 
                        ? GetCellValue(workbookPart, cells[columnIndex])?.Trim() ?? ""
                        : "";
                    rowData[mapping.Key] = cellValue;
                    columnIndex++;
                }

                var mappedObject = mapFunction(rowData);
                if (mappedObject != null)
                {
                    result.Add(mappedObject);
                }
            }

            return result;
        }

        public byte[] GenerateTemplate(Dictionary<string, string> columnMappings, string sheetName, Dictionary<string, string[]>? validationValues = null, List<Dictionary<string, string>>? exampleData = null)
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
                    Name = sheetName
                });

                // Crear fila de encabezados
                var headerRow = new Row { RowIndex = 1 };
                var columnIndex = 1;
                foreach (var mapping in columnMappings)
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

                // Agregar datos de ejemplo si se proporcionan
                if (exampleData != null && exampleData.Any())
                {
                    var rowIndex = 2;
                    foreach (var example in exampleData)
                    {
                        var exampleRow = new Row { RowIndex = (uint)rowIndex };
                        columnIndex = 1;

                        foreach (var mapping in columnMappings)
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