using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Highdmin.Data;
using Highdmin.Models;
using Highdmin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;
using System.Text;

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

        // POST: Pacientes/ImportarPlantilla
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportarPlantilla(ImportarPacientesViewModel viewModel)
        {
            if (!ModelState.IsValid || viewModel.ArchivoExcel == null)
            {
                return View(viewModel);
            }

            try
            {
                using var stream = viewModel.ArchivoExcel.OpenReadStream();
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                var pacientesImportados = 0;
                var errores = new List<string>();

                for (int row = 2; row <= rowCount; row++) // Asumiendo que la fila 1 son headers
                {
                    try
                    { 
                        var tipoIdentificacion = worksheet.Cells[row, 1].Text?.Trim();
                        var identificacion = worksheet.Cells[row, 2].Text?.Trim();
                        var primerNombre = worksheet.Cells[row, 3].Text?.Trim();
                        var segundoNombre = worksheet.Cells[row, 4].Text?.Trim();
                        var primerApellido = worksheet.Cells[row, 5].Text?.Trim();
                        var segundoApellido = worksheet.Cells[row, 6].Text?.Trim();
                        var fechaNacimientoText = worksheet.Cells[row, 7].Text?.Trim();
                        var sexo = worksheet.Cells[row, 8].Text?.Trim();
                        var genero = worksheet.Cells[row, 9].Text?.Trim();
                        var eps = worksheet.Cells[row, 10].Text?.Trim();

                        if (string.IsNullOrEmpty(identificacion) ||
                            string.IsNullOrEmpty(primerNombre) || string.IsNullOrEmpty(primerApellido))
                        {
                            errores.Add($"Fila {row}: Campos obligatorios vacíos");
                            continue;
                        }

                        if (!DateTime.TryParse(fechaNacimientoText, out var fechaNacimiento))
                        {
                            errores.Add($"Fila {row}: Fecha de nacimiento inválida");
                            continue;
                        }

                        // Verificar si el paciente ya existe
                        var existePaciente = await _context.Pacientes
                            .AnyAsync(p => p.Identificacion == identificacion);

                        if (existePaciente && !viewModel.SobrescribirDatos)
                        {
                            errores.Add($"Fila {row}: Paciente con identificación {identificacion} ya existe");
                            continue;
                        }

                        if (existePaciente && viewModel.SobrescribirDatos)
                        {
                            // Actualizar paciente existente
                            var pacienteExistente = await _context.Pacientes
                                .FirstAsync(p => p.Identificacion == identificacion);
                            pacienteExistente.TipoIdentificacion = tipoIdentificacion;
                            pacienteExistente.Identificacion = identificacion;
                            pacienteExistente.PrimerNombre = primerNombre;
                            pacienteExistente.SegundoNombre = segundoNombre;
                            pacienteExistente.PrimerApellido = primerApellido;
                            pacienteExistente.SegundoApellido = segundoApellido;
                            pacienteExistente.FechaNacimiento = fechaNacimiento;
                            pacienteExistente.Sexo = sexo ?? "";
                            pacienteExistente.Genero = genero ?? "";
                            pacienteExistente.FechaActualizacion = DateTime.Now;
                            pacienteExistente.Eps = eps;

                            _context.Update(pacienteExistente);
                        }
                        else
                        {
                            // Crear nuevo paciente
                            var nuevoPaciente = new Paciente
                            {
                                Eps = eps,
                                Identificacion = identificacion,
                                TipoIdentificacion = tipoIdentificacion,
                                PrimerNombre = primerNombre,
                                SegundoNombre = segundoNombre,
                                PrimerApellido = primerApellido,
                                SegundoApellido = segundoApellido,
                                FechaNacimiento = fechaNacimiento,
                                Sexo = sexo ?? "",
                                Genero = genero ?? "",
                                Estado = true,
                                FechaCreacion = DateTime.Now
                            };

                            _context.Add(nuevoPaciente);
                        }

                        pacientesImportados++;
                    }
                    catch (Exception ex)
                    {
                        errores.Add($"Fila {row}: {ex.Message}");
                    }
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = $"Se importaron {pacientesImportados} pacientes exitosamente.";

                if (errores.Any())
                {
                    TempData["Warning"] = $"Se encontraron {errores.Count} errores durante la importación.";
                    TempData["Errores"] = string.Join("<br>", errores);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al importar pacientes desde Excel");
                ModelState.AddModelError("", "Error al procesar el archivo Excel. Verifique el formato.");
                return View(viewModel);
            }
        }

        // GET: Pacientes/DescargarPlantilla
        public IActionResult DescargarPlantilla()
        {
            try
            {
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Pacientes");

                // Headers
                worksheet.Cells[1, 1].Value = "Tipo Identificación";
                worksheet.Cells[1, 2].Value = "Identificación";
                worksheet.Cells[1, 3].Value = "Primer Nombre";
                worksheet.Cells[1, 4].Value = "Segundo Nombre";
                worksheet.Cells[1, 5].Value = "Primer Apellido";
                worksheet.Cells[1, 6].Value = "Segundo Apellido";
                worksheet.Cells[1, 7].Value = "Fecha Nacimiento";
                worksheet.Cells[1, 8].Value = "Sexo";
                worksheet.Cells[1, 9].Value = "Genero";

                // Formatear headers
                using (var range = worksheet.Cells[1, 1, 1, 9])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Ejemplo de datos
                worksheet.Cells[2, 1].Value = "CC";
                worksheet.Cells[2, 2].Value = "12345678";
                worksheet.Cells[2, 3].Value = "Juan";
                worksheet.Cells[2, 4].Value = "Carlos";
                worksheet.Cells[2, 5].Value = "Pérez";
                worksheet.Cells[2, 6].Value = "López";
                worksheet.Cells[2, 7].Value = "1990-01-15";
                worksheet.Cells[2, 8].Value = "Masculino";
                worksheet.Cells[2, 8].Value = "Masculino"; 

                worksheet.Cells.AutoFitColumns();

                var content = package.GetAsByteArray();
                return File(content, 
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    "PlantillaPacientes.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar plantilla de pacientes");
                TempData["Error"] = "Error al generar la plantilla.";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool PacienteExists(int id)
        {
            return _context.Pacientes.Any(e => e.Id == id);
        }
    }
}