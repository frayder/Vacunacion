using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Highdmin.Data;
using Highdmin.Models;
using Highdmin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text;
using System.Globalization; // 👈 ESTA LÍNEA ES LA CLAVE

namespace Highdmin.Controllers
{
    [Authorize]
    public class PacientesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PacientesController> _logger;

        public PacientesController(ApplicationDbContext context, ILogger<PacientesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Pacientes
        public async Task<IActionResult> Index(string? buscar, string? epsFilter)
        {
            try
            {
                var query = _context.Pacientes.AsQueryable();

                // Aplicar filtros
                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    query = query.Where(p => p.NombreCompleto.Contains(buscar) ||
                                           p.Identificacion.Contains(buscar));
                }

                if (!string.IsNullOrWhiteSpace(epsFilter))
                {
                    query = query.Where(p => p.Eps.Contains(epsFilter));
                }

                var pacientes = await query
                    .OrderByDescending(p => p.FechaCreacion)
                    .Select(p => new PacienteItemViewModel
                    {
                        Id = p.Id,
                        Eps = p.Eps,
                        Identificacion = p.Identificacion,
                        PrimerNombre = p.PrimerNombre,
                        SegundoNombre = p.SegundoNombre,
                        PrimerApellido = p.PrimerApellido,
                        SegundoApellido = p.SegundoApellido,
                        TipoIdentificacion = p.TipoIdentificacion,
                        FechaNacimiento = p.FechaNacimiento,
                        Sexo = p.Sexo,
                        Estado = p.Estado,
                        FechaCreacion = p.FechaCreacion
                    })
                    .ToListAsync();

                var totalPacientes = await _context.Pacientes.CountAsync();
                var pacientesConDatos = await _context.Pacientes.CountAsync(p => p.Estado);

                var viewModel = new PacienteViewModel
                {
                    TotalPacientes = totalPacientes,
                    PacientesConDatos = pacientesConDatos,
                    CargasRealizadas = 0, // TODO: Implementar lógica de cargas
                    Pacientes = pacientes,
                    HistorialCargas = new List<CargaHistorialItem>() // TODO: Implementar historial
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la lista de pacientes");
                TempData["Error"] = "Error al cargar los datos de pacientes.";
                return View(new PacienteViewModel());
            }
        }

        // GET: Pacientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paciente = await _context.Pacientes
                .Include(p => p.RegistrosVacunacion)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (paciente == null)
            {
                return NotFound();
            }

            return View(paciente);
        }

        // GET: Pacientes/Create
        public IActionResult Create()
        {
            return View(new PacienteCreateViewModel());
        }

        // POST: Pacientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PacienteCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si ya existe un paciente con la misma identificación
                    var existePaciente = await _context.Pacientes
                        .AnyAsync(p => p.Identificacion == viewModel.Identificacion);

                    if (existePaciente)
                    {
                        ModelState.AddModelError("Identificacion", "Ya existe un paciente con esta identificación.");
                        return View(viewModel);
                    }

                    var paciente = new Paciente
                    {
                        Eps = viewModel.Eps,
                        Identificacion = viewModel.Identificacion,
                        TipoIdentificacion = viewModel.TipoIdentificacion,
                        PrimerNombre = viewModel.PrimerNombre,
                        SegundoNombre = viewModel.SegundoNombre,
                        PrimerApellido = viewModel.PrimerApellido,
                        SegundoApellido = viewModel.SegundoApellido,
                        FechaNacimiento = viewModel.FechaNacimiento,
                        Sexo = viewModel.Sexo,
                        Genero = viewModel.Genero,
                        Telefono = viewModel.Telefono,
                        Email = viewModel.Email,
                        Estado = viewModel.Estado,
                        FechaCreacion = DateTime.Now
                    };

                    _context.Add(paciente);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Paciente creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear paciente");
                    ModelState.AddModelError("", "Error al crear el paciente. Intente nuevamente.");
                }
            }

            return View(viewModel);
        }

        // GET: Pacientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paciente = await _context.Pacientes.FindAsync(id);
            if (paciente == null)
            {
                return NotFound();
            }

            var viewModel = new PacienteEditViewModel
            {
                Id = paciente.Id,
                Eps = paciente.Eps,
                Identificacion = paciente.Identificacion,
                TipoIdentificacion = paciente.TipoIdentificacion,
                PrimerNombre = paciente.PrimerNombre,
                SegundoNombre = paciente.SegundoNombre,
                PrimerApellido = paciente.PrimerApellido,
                SegundoApellido = paciente.SegundoApellido,
                FechaNacimiento = paciente.FechaNacimiento,
                Sexo = paciente.Sexo,
                Telefono = paciente.Telefono,
                Email = paciente.Email,
                Genero = paciente.Genero,
                Estado = paciente.Estado,
                FechaCreacion = paciente.FechaCreacion
            };

            return View(viewModel);
        }

        // POST: Pacientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PacienteEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si ya existe otro paciente con la misma identificación
                    var existeOtroPaciente = await _context.Pacientes
                        .AnyAsync(p => p.Identificacion == viewModel.Identificacion && p.Id != id);

                    if (existeOtroPaciente)
                    {
                        ModelState.AddModelError("Identificacion", "Ya existe otro paciente con esta identificación.");
                        return View(viewModel);
                    }

                    var paciente = await _context.Pacientes.FindAsync(id);
                    if (paciente == null)
                    {
                        return NotFound();
                    }

                    paciente.Eps = viewModel.Eps;
                    paciente.Identificacion = viewModel.Identificacion;
                    paciente.TipoIdentificacion = paciente.TipoIdentificacion;
                    paciente.PrimerNombre = viewModel.PrimerNombre;
                    paciente.SegundoNombre = viewModel.SegundoNombre;
                    paciente.PrimerApellido = viewModel.PrimerApellido;
                    paciente.SegundoApellido = viewModel.SegundoApellido;
                    paciente.FechaNacimiento = viewModel.FechaNacimiento;
                    paciente.Sexo = viewModel.Sexo;
                    paciente.Telefono = viewModel.Telefono;
                    paciente.Email = viewModel.Email;
                    paciente.Genero = viewModel.Genero;
                    paciente.Estado = viewModel.Estado;
                    paciente.FechaActualizacion = DateTime.Now;

                    _context.Update(paciente);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Paciente actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PacienteExists(viewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar paciente {Id}", id);
                    ModelState.AddModelError("", "Error al actualizar el paciente. Intente nuevamente.");
                }
            }

            return View(viewModel);
        }

        // GET: Pacientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(m => m.Id == id);

            if (paciente == null)
            {
                return NotFound();
            }

            return View(paciente);
        }

        // POST: Pacientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var paciente = await _context.Pacientes.FindAsync(id);
                if (paciente != null)
                {
                    _context.Pacientes.Remove(paciente);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Paciente eliminado exitosamente.";
                }
                else
                {
                    TempData["Error"] = "No se encontró el paciente a eliminar.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar paciente {Id}", id);
                TempData["Error"] = "Error al eliminar el paciente. Puede que tenga registros asociados.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Pacientes/ImportarPlantilla
        public IActionResult ImportarPlantilla()
        {
            return View(new ImportarPacientesViewModel());
        }

        // GET: Pacientes/DescargarPlantilla
        [HttpGet]
        [ActionName("DescargarPlantilla")]
        public IActionResult DescargarPlantilla()
        {
            try
            {
                _logger.LogInformation("Iniciando descarga de plantilla de pacientes");

                // Definir encabezados y valores de validación
                var headers = new List<(string Name, string[] ValidationValues)>
        {
            ("Tipo Identificación", new[] { "CC", "TI", "CE", "PA", "RC" }),
            ("Identificación", Array.Empty<string>()),
            ("Primer Nombre", Array.Empty<string>()),
            ("Segundo Nombre", Array.Empty<string>()),
            ("Primer Apellido", Array.Empty<string>()),
            ("Segundo Apellido", Array.Empty<string>()),
            ("Fecha Nacimiento", Array.Empty<string>()),
            ("Sexo", new[] { "Masculino", "Femenino" }),
            ("EPS", Array.Empty<string>())
        };

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
                        Name = "Pacientes"
                    });

                    // Fila de encabezados
                    var headerRow = new Row();
                    for (int i = 0; i < headers.Count; i++)
                    {
                        headerRow.Append(CreateTextCell(headers[i].Name));
                    }
                    sheetData.Append(headerRow);

                    // Fila de ejemplo
                    var exampleRow = new Row();
                    var exampleData = new[]
                    {
                "CC", "12345678", "Juan", "Carlos", "Pérez", "López",
                "1990-01-15", "Masculino", "EPS_EJEMPLO"
            };
                    foreach (var cellValue in exampleData)
                    {
                        exampleRow.Append(CreateTextCell(cellValue));
                    }
                    sheetData.Append(exampleRow);

                    // Agregar validaciones de lista (DataValidation)
                    var dataValidations = new DataValidations();
                    for (int i = 0; i < headers.Count; i++)
                    {
                        var validationValues = headers[i].ValidationValues;
                        if (validationValues != null && validationValues.Length > 0)
                        {
                            var formula = "\"" + string.Join(",", validationValues) + "\"";
                            var columnLetter = GetExcelColumnName(i + 1);
                            var validation = new DataValidation
                            {
                                Type = DataValidationValues.List,
                                AllowBlank = true,
                                ShowInputMessage = true,
                                ShowErrorMessage = true,
                                SequenceOfReferences = new ListValue<StringValue> { InnerText = $"{columnLetter}2:{columnLetter}1000" },
                                Formula1 = new Formula1(formula)
                            };
                            dataValidations.Append(validation);
                        }
                    }

                    worksheetPart.Worksheet.Append(dataValidations);
                    workbookPart.Workbook.Save();
                }

                // Generar bytes del archivo
                var content = stream.ToArray();
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = $"Plantilla_Pacientes_{DateTime.Now:yyyyMMdd}.xlsx";

                _logger.LogInformation($"Plantilla generada exitosamente: {fileName}");

                return File(content, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar la plantilla de pacientes");
                TempData["Error"] = "Error al generar la plantilla de pacientes.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ActionName("ImportarPlantilla")]
      public IActionResult ImportarPlantilla(ImportarPacientesViewModel model)
{
    if (model.ArchivoExcel == null || model.ArchivoExcel.Length == 0)
    {
        ModelState.AddModelError("ArchivoExcel", "Debe seleccionar un archivo Excel válido.");
        return View(model);
    }

    try
    {
        using var stream = model.ArchivoExcel.OpenReadStream();
        using var document = SpreadsheetDocument.Open(stream, false);
        var workbookPart = document.WorkbookPart;
        var sheet = workbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
        var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
        var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

        var pacientes = new List<PacienteItemViewModel>();

        // Saltamos la primera fila (encabezados)
        foreach (var row in sheetData.Elements<Row>().Skip(1))
        {
            var cells = row.Elements<Cell>().ToList();

            var paciente = new PacienteItemViewModel
            {
                TipoIdentificacion = GetCellValue(workbookPart, cells.ElementAtOrDefault(0)),
                Identificacion = GetCellValue(workbookPart, cells.ElementAtOrDefault(1)),
                PrimerNombre = GetCellValue(workbookPart, cells.ElementAtOrDefault(2)),
                SegundoNombre = GetCellValue(workbookPart, cells.ElementAtOrDefault(3)),
                PrimerApellido = GetCellValue(workbookPart, cells.ElementAtOrDefault(4)),
                SegundoApellido = GetCellValue(workbookPart, cells.ElementAtOrDefault(5)),
                FechaNacimiento = ParseDate(GetCellValue(workbookPart, cells.ElementAtOrDefault(6))) ?? DateTime.MinValue,
                Sexo = GetCellValue(workbookPart, cells.ElementAtOrDefault(7)),
                Eps = GetCellValue(workbookPart, cells.ElementAtOrDefault(8))
            };

            // Si se definió filtro por EPS
            if (!string.IsNullOrWhiteSpace(model.EpsFilter) &&
                !paciente.Eps?.Contains(model.EpsFilter, StringComparison.OrdinalIgnoreCase) == true)
                continue;

            pacientes.Add(paciente);
        }

        model.PacientesCargados = pacientes;

        TempData["Success"] = $"Se importaron {pacientes.Count} pacientes correctamente.";
        return View(model);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al importar la plantilla de pacientes");
        TempData["Error"] = "Error al procesar el archivo Excel.";
        return View(model);
    }
}

        private bool PacienteExists(int id)
        {
            return _context.Pacientes.Any(e => e.Id == id);
        }

        private static Cell CreateTextCell(string text)
        {
            return new Cell
            {
                DataType = CellValues.String,
                CellValue = new CellValue(text)
            };
        }

        private static string GetCellValue(WorkbookPart workbookPart, Cell cell)
        {
            if (cell == null) return string.Empty;

            string value = cell.InnerText;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                var stringTable = workbookPart.SharedStringTablePart.SharedStringTable;
                value = stringTable.ElementAt(int.Parse(value)).InnerText;
            }
            return value.Trim();
        }

        private static DateTime? ParseDate(string value)
        {
            if (DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;
            if (DateTime.TryParse(value, out var date2))
                return date2;
            return null;
        }

        private static string GetExcelColumnName(int columnNumber)
        {
            var columnName = string.Empty;
            while (columnNumber > 0)
            {
                var modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }
            return columnName;
        }
    }
}