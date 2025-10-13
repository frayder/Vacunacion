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
using System.Globalization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Highdmin.Services;

namespace Highdmin.Controllers
{
    [Authorize]
    public class PacientesController : BaseEmpresaController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PacientesController> _logger;
        private static List<PacienteItemViewModel> PacientesCargados = new List<PacienteItemViewModel>();

        public PacientesController(ApplicationDbContext context, ILogger<PacientesController> logger, IEmpresaService empresaService) : base(empresaService)
        {
            _context = context;
            _logger = logger;
        }
        private static string GetValue(WorkbookPart workbookPart, Cell? cell)
        {
            if (cell == null)
                return string.Empty;

            string value = cell.InnerText ?? string.Empty;

            // Si la celda tiene un tipo de dato explícito
            if (cell.DataType != null)
            {
                var dataType = cell.DataType.Value;

                if (dataType == CellValues.SharedString)
                {
                    if (int.TryParse(value, out var sstIndex) &&
                        workbookPart.SharedStringTablePart?.SharedStringTable != null)
                    {
                        var sstItem = workbookPart.SharedStringTablePart.SharedStringTable
                            .Elements<SharedStringItem>()
                            .ElementAtOrDefault(sstIndex);

                        if (sstItem != null)
                            value = sstItem.InnerText ?? string.Empty;
                    }
                }
                else if (dataType == CellValues.Boolean)
                {
                    value = value == "1" ? "TRUE" : "FALSE";
                }
            }
            else
            {
                // Si no tiene tipo explícito, puede ser numérico o fecha
                if (double.TryParse(value, out double numericValue))
                {
                    if (cell.StyleIndex != null)
                    {
                        var stylesPart = workbookPart.WorkbookStylesPart;
                        if (stylesPart != null)
                        {
                            var cellFormat = stylesPart.Stylesheet.CellFormats
                                .ElementAtOrDefault((int)cell.StyleIndex.Value) as CellFormat;

                            // Detectar formato de fecha por su NumberFormatId
                            if (cellFormat != null && cellFormat.NumberFormatId != null)
                            {
                                uint numFmtId = cellFormat.NumberFormatId.Value;
                                if (numFmtId >= 14 && numFmtId <= 22)
                                {
                                    value = DateTime.FromOADate(numericValue).ToString("yyyy-MM-dd");
                                }
                            }
                        }
                    }
                }
            }

            return value.Trim();
        }

        private static DateTime? ParseDate(string? value)
        {
            if (string.IsNullOrEmpty(value)) return null;

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

        private static Cell CreateTextCell(string text)
        {
            return new Cell
            {
                DataType = CellValues.String,
                CellValue = new CellValue(text ?? string.Empty)
            };
        }

        private bool PacienteExists(int id)
        {
            return _context.Pacientes.Any(e => e.Id == id && e.EmpresaId == CurrentEmpresaId);
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
                    .Where(p => p.EmpresaId == CurrentEmpresaId)
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
                var historialCargas = await _context.HistorialCargas
                    .Where(h => h.EmpresaId == CurrentEmpresaId)
                    .OrderByDescending(h => h.FechaCarga)
                    .Select(h => new CargaHistorialItem
                    {
                        Eps = h.Eps,
                        Archivo = h.ArchivoNombre ?? "No disponible",
                        FechaCarga = h.FechaCarga,
                        Registros = h.TotalCargados,
                        Acciones = h.Observaciones ?? string.Empty
                    })
                    .ToListAsync();

                var viewModel = new PacienteViewModel
                {
                    TotalPacientes = totalPacientes,
                    PacientesConDatos = pacientesConDatos,
                    CargasRealizadas = historialCargas.Count,
                    Pacientes = pacientes,
                    HistorialCargas = historialCargas
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
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

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
                        FechaCreacion = DateTime.Now,
                        EmpresaId = CurrentEmpresaId
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

        [HttpPost]
        public async Task<IActionResult> GuardarPacientesImportados()
        {
            var json = HttpContext.Session.GetString("PacientesImportados");

            if (string.IsNullOrEmpty(json))
                return Json(new { success = false, message = "No hay pacientes para guardar." });

            var pacientesCargados = JsonConvert.DeserializeObject<List<Paciente>>(json);
            try
            {
                int nuevos = 0, existentes = 0;
                foreach (var p in pacientesCargados)
                {
                    var pacienteExistente = await _context.Pacientes
                        .FirstOrDefaultAsync(x => x.Identificacion == p.Identificacion && x.EmpresaId == CurrentEmpresaId);
                    if (pacienteExistente != null)
                    {
                        // 🔁 Actualizar información del paciente existente
                        pacienteExistente.TipoIdentificacion = p.TipoIdentificacion ?? pacienteExistente.TipoIdentificacion;
                        pacienteExistente.PrimerNombre = p.PrimerNombre ?? pacienteExistente.PrimerNombre;
                        pacienteExistente.SegundoNombre = p.SegundoNombre ?? pacienteExistente.SegundoNombre;
                        pacienteExistente.PrimerApellido = p.PrimerApellido ?? pacienteExistente.PrimerApellido;
                        pacienteExistente.SegundoApellido = p.SegundoApellido ?? pacienteExistente.SegundoApellido;
                        pacienteExistente.FechaNacimiento = p.FechaNacimiento;
                        pacienteExistente.Sexo = p.Sexo ?? pacienteExistente.Sexo;
                        pacienteExistente.Genero = p.Genero ?? pacienteExistente.Genero;
                        pacienteExistente.Eps = p.Eps ?? pacienteExistente.Eps;
                        pacienteExistente.Estado = true;
                        pacienteExistente.FechaActualizacion = DateTime.Now; // Si tienes este campo en tu modelo

                        _context.Pacientes.Update(pacienteExistente);
                        existentes++;
                        continue;
                    }

                    var paciente = new Paciente
                    {
                        TipoIdentificacion = p.TipoIdentificacion ?? "CC",
                        Identificacion = p.Identificacion,
                        PrimerNombre = p.PrimerNombre,
                        SegundoNombre = p.SegundoNombre,
                        PrimerApellido = p.PrimerApellido,
                        SegundoApellido = p.SegundoApellido,
                        FechaNacimiento = p.FechaNacimiento,
                        Sexo = p.Sexo,
                        Genero = p.Genero,
                        Eps = p.Eps,
                        Estado = true,
                        FechaCreacion = DateTime.Now
                    };

                    _context.Pacientes.Add(paciente);
                    nuevos++;
                }

                await _context.SaveChangesAsync();

                // Guardar historial de carga
                var usuario = User.Identity?.Name ?? "Sistema";
                var historial = new HistorialCargaPacientes
                {
                    Usuario = usuario,
                    FechaCarga = DateTime.Now,
                    TotalCargados = nuevos,
                    TotalExistentes = existentes,
                    ArchivoNombre = "Carga desde Excel",
                    Observaciones = $"{nuevos} nuevos, {existentes} ya existían."
                };
                _context.HistorialCargas.Add(historial);
                await _context.SaveChangesAsync();

                PacientesCargados.Clear();
                HttpContext.Session.Remove("PacientesImportados");

                return Json(new { success = true, message = $"{nuevos} pacientes guardados. {existentes} ya existían." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar pacientes importados");
                return Json(new { success = false, message = "Ocurrió un error al guardar los pacientes." });
            }
        }
        // GET: Pacientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paciente = await _context.Pacientes.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
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
                        .AnyAsync(p => p.Identificacion == viewModel.Identificacion && p.Id != id && p.EmpresaId == CurrentEmpresaId);

                    if (existeOtroPaciente)
                    {
                        ModelState.AddModelError("Identificacion", "Ya existe otro paciente con esta identificación.");
                        return View(viewModel);
                    }

                    var paciente = await _context.Pacientes.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
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
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

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
                var paciente = await _context.Pacientes.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
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
        public async Task<IActionResult> ImportarPlantilla()
        {
            // Obtener la lista de aseguradoras activas ordenadas por nombre
            var aseguradoras = await _context.Aseguradoras
                .Where(a => a.Estado)
                .OrderBy(a => a.Nombre)
                .Select(a => new SelectListItem
                {
                    Value = a.Nombre,
                    Text = $"{a.Nombre} ({a.Codigo})"
                })
                .ToListAsync();

            ViewBag.Aseguradoras = aseguradoras;
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
                    ("Sexo", new[] { "Masculino", "Femenino" })
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
        public async Task<IActionResult> ImportarPlantilla(ImportarPacientesViewModel model)
        {

            try
            {
                using var stream = model.ArchivoExcel.OpenReadStream();
                using var document = SpreadsheetDocument.Open(stream, false);

                if (document.WorkbookPart == null)
                {
                    ModelState.AddModelError("", "El archivo Excel no es válido.");
                    return View(model);
                }

                var workbookPart = document.WorkbookPart;
                if (workbookPart.Workbook?.Sheets == null)
                {
                    ModelState.AddModelError("", "El archivo Excel no contiene hojas de cálculo.");
                    return View(model);
                }

                var sheet = workbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
                if (sheet == null)
                {
                    ModelState.AddModelError("", "No se encontró ninguna hoja en el archivo Excel.");
                    return View(model);
                }

                var sheetId = sheet.Id?.Value;
                if (string.IsNullOrEmpty(sheetId))
                {
                    ModelState.AddModelError("", "La referencia a la hoja de cálculo no es válida.");
                    return View(model);
                }

                var worksheetPart = workbookPart.GetPartById(sheetId) as WorksheetPart;
                if (worksheetPart?.Worksheet == null)
                {
                    ModelState.AddModelError("", "La estructura del archivo Excel no es válida.");
                    return View(model);
                }

                var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                if (sheetData == null)
                {
                    ModelState.AddModelError("", "El archivo Excel está vacío.");
                    return View(model);
                }

                model.PacientesCargados = new List<PacienteItemViewModel>();
                var dbPacientes = new List<Paciente>();

                // Saltamos la primera fila (encabezados)
                foreach (var row in sheetData.Elements<Row>().Skip(1))
                {
                    var cells = row.Elements<Cell>().ToList();
                    if (!cells.Any()) continue;

                    string tipoIdent = GetValue(workbookPart, GetCell(row, "A"));
                    string identificacion = GetValue(workbookPart, GetCell(row, "B"));
                    string primerNombre = GetValue(workbookPart, GetCell(row, "C"));
                    string segundoNombre = GetValue(workbookPart, GetCell(row, "D"));
                    string primerApellido = GetValue(workbookPart, GetCell(row, "E"));
                    string segundoApellido = GetValue(workbookPart, GetCell(row, "F"));
                    string fechaNacStr = GetValue(workbookPart, GetCell(row, "G"));
                    string sexo = GetValue(workbookPart, GetCell(row, "H"));
                    string eps = model.Eps;

                    var paciente = new Paciente
                    {
                        TipoIdentificacion = tipoIdent ?? "CC",
                        Identificacion = identificacion,
                        PrimerNombre = primerNombre,
                        SegundoNombre = segundoNombre,
                        PrimerApellido = primerApellido,
                        SegundoApellido = segundoApellido,
                        FechaNacimiento = ParseDate(fechaNacStr) ?? DateTime.MinValue,
                        Sexo = sexo,
                        Eps = eps,
                        Estado = true,
                        FechaCreacion = DateTime.Now,
                        EmpresaId = CurrentEmpresaId
                    };

                    // Solo agregar si tiene los campos requeridos
                    if (string.IsNullOrWhiteSpace(paciente.Identificacion) ||
                        string.IsNullOrWhiteSpace(paciente.PrimerNombre) ||
                        string.IsNullOrWhiteSpace(paciente.PrimerApellido))
                    {
                        continue;
                    }

                    dbPacientes.Add(paciente);

                    // Agregar al modelo de vista
                    model.PacientesCargados.Add(new PacienteItemViewModel
                    {
                        TipoIdentificacion = paciente.TipoIdentificacion,
                        Identificacion = paciente.Identificacion,
                        PrimerNombre = paciente.PrimerNombre,
                        SegundoNombre = paciente.SegundoNombre ?? "",
                        PrimerApellido = paciente.PrimerApellido,
                        SegundoApellido = paciente.SegundoApellido ?? "",
                        FechaNacimiento = paciente.FechaNacimiento,
                        Sexo = paciente.Sexo,
                        Genero = paciente.Genero,
                        Eps = paciente.Eps
                    });
                }
                if (!model.PacientesCargados.Any())
                {
                    ModelState.AddModelError("", "No se encontraron registros válidos en el archivo Excel.");
                    return View(model);
                }
                
                HttpContext.Session.SetString("PacientesImportados", JsonConvert.SerializeObject(model.PacientesCargados));
                TempData["Success"] = $"Se importaron {model.PacientesCargados.Count} pacientes correctamente.";
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al importar la plantilla de pacientes");
                ModelState.AddModelError("", "Error al procesar el archivo Excel. Por favor, verifique el formato del archivo.");
                return View(model);
            }
        }

        private static Cell? GetCell(Row row, string columnName)
        {
            return row.Elements<Cell>()
                      .FirstOrDefault(c => c.CellReference?.Value.StartsWith(columnName) == true);
        }
    }
}