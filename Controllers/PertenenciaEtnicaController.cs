using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Highdmin.Data;
using Highdmin.Models;
using Highdmin.ViewModels;
using Highdmin.Services;
using System.Security.Claims;
using Newtonsoft.Json;

namespace Highdmin.Controllers
{
    public class PertenenciaEtnicaController : BaseAuthorizationController
    {
        private readonly ApplicationDbContext _context;
        private readonly IImportExportService _importExportService;
        private readonly IEntityConfigurationService _configurationService;
        private readonly IDataPersistenceService _persistenceService;

        public PertenenciaEtnicaController(
            ApplicationDbContext context, 
            IEmpresaService empresaService, 
            AuthorizationService authorizationService,
            IImportExportService importExportService,
            IEntityConfigurationService configurationService,
            IDataPersistenceService persistenceService) 
            : base(empresaService, authorizationService)
        {
            _context = context;
            _importExportService = importExportService;
            _configurationService = configurationService;
            _persistenceService = persistenceService;
        }

        // GET: PertenenciaEtnica
        public async Task<IActionResult> Index()
        {
            try
            {
                var (redirect, permissions) = await ValidateAndGetPermissionsAsync("PertenenciaEtnica", "Read");
                if (redirect != null) return redirect;

                var pertenencias = await _context.PertenenciasEtnicas
                    .Where(p => p.EmpresaId == CurrentEmpresaId)
                    .OrderBy(p => p.Codigo)
                    .Select(p => new PertenenciaEtnicaItemViewModel
                    {
                        Id = p.Id,
                        Codigo = p.Codigo,
                        Nombre = p.Nombre,
                        Descripcion = p.Descripcion,
                        Estado = p.Estado,
                        FechaCreacion = p.FechaCreacion
                    })
                    .ToListAsync();

                var viewModel = new PertenenciaEtnicaViewModel
                {
                    TotalPertenencias = pertenencias.Count,
                    PertenenciasActivas = pertenencias.Count(p => p.Estado),
                    PertenenciasInactivas = pertenencias.Count(p => !p.Estado),
                    PertenenciasEtnicas = pertenencias,
                    CanCreate = permissions["Create"],
                    CanUpdate = permissions["Update"],
                    CanDelete = permissions["Delete"]
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar las pertenencias étnicas: " + ex.Message;
                return View(new PertenenciaEtnicaViewModel());
            }
        }

        // GET: PertenenciaEtnica/Exportar
        public async Task<IActionResult> Exportar()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasExportPermission = await _authorizationService.HasPermissionAsync(userId, "PertenenciaEtnica", "Export") || 
                                    await _authorizationService.HasPermissionAsync(userId, "PertenenciaEtnica", "Read");
            
            if (!hasExportPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para exportar pertenencias étnicas.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var pertenencias = await _context.PertenenciasEtnicas
                    .Where(p => p.EmpresaId == CurrentEmpresaId)
                    .OrderBy(p => p.Codigo)
                    .ToListAsync();

                var exportConfig = _configurationService.GetExportConfiguration<PertenenciaEtnica>();
                var excelData = await _importExportService.ExportToExcelAsync(pertenencias, exportConfig);
                var fileName = $"{exportConfig.FileName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al exportar las pertenencias étnicas: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: PertenenciaEtnica/ImportarPlantilla
        public async Task<IActionResult> ImportarPlantilla()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasImportPermission = await _authorizationService.HasPermissionAsync(userId, "PertenenciaEtnica", "Import") || 
                                    await _authorizationService.HasPermissionAsync(userId, "PertenenciaEtnica", "Create");
            
            if (!hasImportPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para importar pertenencias étnicas.";
                return RedirectToAction(nameof(Index));
            }

            return View(new ImportarPertenenciaEtnicaViewModel());
        }

        // GET: PertenenciaEtnica/DescargarPlantilla
        [HttpGet]
        [ActionName("DescargarPlantilla")]
        public IActionResult DescargarPlantilla()
        {
            try
            {
                var importConfig = _configurationService.GetImportConfiguration<PertenenciaEtnicaItemViewModel>();
                var templateData = _importExportService.GenerateImportTemplate(importConfig);
                var fileName = $"Plantilla_{importConfig.SheetName.Replace(" ", "")}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(templateData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al generar la plantilla: " + ex.Message;
                return RedirectToAction(nameof(ImportarPlantilla));
            }
        }

        // POST: PertenenciaEtnica/ImportarPlantilla
        [HttpPost]
        [ActionName("ImportarPlantilla")]
        public async Task<IActionResult> ImportarPlantilla(ImportarPertenenciaEtnicaViewModel model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasImportPermission = await _authorizationService.HasPermissionAsync(userId, "PertenenciaEtnica", "Import") || 
                                    await _authorizationService.HasPermissionAsync(userId, "PertenenciaEtnica", "Create");
            
            if (!hasImportPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para importar pertenencias étnicas.";
                return RedirectToAction(nameof(Index));
            }

            if (model.ArchivoExcel == null || model.ArchivoExcel.Length == 0)
            {
                ModelState.AddModelError("ArchivoExcel", "Debe seleccionar un archivo Excel.");
                return View(model);
            }

            try
            {
                var importConfig = _configurationService.GetImportConfiguration<PertenenciaEtnicaItemViewModel>();
                var importResult = await _importExportService.ImportFromExcelAsync(model.ArchivoExcel, importConfig);

                if (importResult.HasErrors)
                {
                    ViewBag.Errores = importResult.Errors;
                    return View(model);
                }

                if (!importResult.Data.Any())
                {
                    ModelState.AddModelError("", "No se encontraron datos válidos para importar.");
                    return View(model);
                }

                HttpContext.Session.SetString("PertenenciasCargadas", JsonConvert.SerializeObject(importResult.Data));
                model.PertenenciasCargadas = importResult.Data;
                TempData["Success"] = $"Se procesaron {importResult.Data.Count} pertenencias étnicas correctamente. Revise los datos y confirme la importación.";

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al procesar el archivo: " + ex.Message);
                return View(model);
            }
        }

        // POST: PertenenciaEtnica/GuardarPertenenciasImportadas
        [HttpPost]
        public async Task<IActionResult> GuardarPertenenciasImportadas()
        {
            var json = HttpContext.Session.GetString("PertenenciasCargadas");
            if (string.IsNullOrEmpty(json))
            {
                TempData["Error"] = "No hay datos para importar.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var pertenenciasCargadas = JsonConvert.DeserializeObject<List<PertenenciaEtnicaItemViewModel>>(json);
                if (pertenenciasCargadas == null || !pertenenciasCargadas.Any())
                {
                    TempData["Error"] = "No hay datos para importar.";
                    return RedirectToAction(nameof(Index));
                }

                var totalProcessed = await _persistenceService.SaveImportedDataAsync<PertenenciaEtnica, PertenenciaEtnicaItemViewModel>(
                    pertenenciasCargadas,
                    CurrentEmpresaId,
                    // Create mapper
                    viewModel => new PertenenciaEtnica
                    {
                        Codigo = viewModel.Codigo.ToUpper(),
                        Nombre = viewModel.Nombre,
                        Descripcion = viewModel.Descripcion,
                        Estado = viewModel.Estado,
                        FechaCreacion = DateTime.Now,
                        EmpresaId = CurrentEmpresaId
                    },
                    // Update mapper
                    (viewModel, existing) =>
                    {
                        existing.Nombre = viewModel.Nombre;
                        existing.Descripcion = viewModel.Descripcion;
                        existing.Estado = viewModel.Estado;
                        return existing;
                    },
                    // Find existing
                    (viewModel, dbSet) => dbSet.FirstOrDefault(p => 
                        p.EmpresaId == CurrentEmpresaId && 
                        p.Codigo.ToUpper() == viewModel.Codigo.ToUpper())
                );

                HttpContext.Session.Remove("PertenenciasCargadas");
                TempData["Success"] = $"Importación completada: {totalProcessed} pertenencias étnicas procesadas exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al guardar las pertenencias étnicas: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: PertenenciaEtnica/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pertenenciaEtnica = await _context.PertenenciasEtnicas
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

            if (pertenenciaEtnica == null)
            {
                return NotFound();
            }

            var viewModel = new PertenenciaEtnicaItemViewModel
            {
                Id = pertenenciaEtnica.Id,
                Codigo = pertenenciaEtnica.Codigo,
                Nombre = pertenenciaEtnica.Nombre,
                Descripcion = pertenenciaEtnica.Descripcion,
                Estado = pertenenciaEtnica.Estado,
                FechaCreacion = pertenenciaEtnica.FechaCreacion
            };

            return View(viewModel);
        }

        // GET: PertenenciaEtnica/Create
        public IActionResult Create()
        {
            return View(new PertenenciaEtnicaCreateViewModel());
        }

        // POST: PertenenciaEtnica/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PertenenciaEtnicaCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el código ya existe
                    var existeCodigo = await _context.PertenenciasEtnicas
                        .AnyAsync(p => p.Codigo == viewModel.Codigo && p.EmpresaId == CurrentEmpresaId);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe una pertenencia étnica con este código.");
                        return View(viewModel);
                    }

                    var pertenenciaEtnica = new PertenenciaEtnica
                    {
                        Codigo = viewModel.Codigo,
                        Nombre = viewModel.Nombre,
                        Descripcion = viewModel.Descripcion,
                        Estado = viewModel.Estado,
                        FechaCreacion = DateTime.Now,
                        EmpresaId = CurrentEmpresaId
                    };

                    _context.Add(pertenenciaEtnica);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Pertenencia étnica creada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al crear la pertenencia étnica: " + ex.Message;
                }
            }

            return View(viewModel);
        }

        // GET: PertenenciaEtnica/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pertenenciaEtnica = await _context.PertenenciasEtnicas.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
            if (pertenenciaEtnica == null)
            {
                return NotFound();
            }

            var viewModel = new PertenenciaEtnicaEditViewModel
            {
                Id = pertenenciaEtnica.Id,
                Codigo = pertenenciaEtnica.Codigo,
                Nombre = pertenenciaEtnica.Nombre,
                Descripcion = pertenenciaEtnica.Descripcion,
                Estado = pertenenciaEtnica.Estado,
                FechaCreacion = pertenenciaEtnica.FechaCreacion
            };

            return View(viewModel);
        }

        // POST: PertenenciaEtnica/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PertenenciaEtnicaEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el código ya existe (excluyendo el registro actual)
                    var existeCodigo = await _context.PertenenciasEtnicas
                        .AnyAsync(p => p.Codigo == viewModel.Codigo && p.Id != id && p.EmpresaId == CurrentEmpresaId);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe una pertenencia étnica con este código.");
                        return View(viewModel);
                    }

                    var pertenenciaEtnica = await _context.PertenenciasEtnicas.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                    if (pertenenciaEtnica == null)
                    {
                        return NotFound();
                    }

                    pertenenciaEtnica.Codigo = viewModel.Codigo;
                    pertenenciaEtnica.Nombre = viewModel.Nombre;
                    pertenenciaEtnica.Descripcion = viewModel.Descripcion;
                    pertenenciaEtnica.Estado = viewModel.Estado;

                    _context.Update(pertenenciaEtnica);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Pertenencia étnica actualizada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PertenenciaEtnicaExists(viewModel.Id))
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
                    TempData["Error"] = "Error al actualizar la pertenencia étnica: " + ex.Message;
                }
            }

            return View(viewModel);
        }

        // GET: PertenenciaEtnica/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pertenenciaEtnica = await _context.PertenenciasEtnicas
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

            if (pertenenciaEtnica == null)
            {
                return NotFound();
            }

            var viewModel = new PertenenciaEtnicaItemViewModel
            {
                Id = pertenenciaEtnica.Id,
                Codigo = pertenenciaEtnica.Codigo,
                Nombre = pertenenciaEtnica.Nombre,
                Descripcion = pertenenciaEtnica.Descripcion,
                Estado = pertenenciaEtnica.Estado,
                FechaCreacion = pertenenciaEtnica.FechaCreacion
            };

            return View(viewModel);
        }

        // POST: PertenenciaEtnica/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var pertenenciaEtnica = await _context.PertenenciasEtnicas.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (pertenenciaEtnica != null)
                {
                    _context.PertenenciasEtnicas.Remove(pertenenciaEtnica);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Pertenencia étnica eliminada exitosamente.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar la pertenencia étnica: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: PertenenciaEtnica/ToggleEstado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            try
            {
                var pertenenciaEtnica = await _context.PertenenciasEtnicas
                    .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (pertenenciaEtnica != null)
                {
                    pertenenciaEtnica.Estado = !pertenenciaEtnica.Estado;
                    _context.Update(pertenenciaEtnica);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, estado = pertenenciaEtnica.Estado });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = false, message = "Pertenencia étnica no encontrada" });
        }

        private bool PertenenciaEtnicaExists(int id)
        {
            return _context.PertenenciasEtnicas.Any(e => e.Id == id && e.EmpresaId == CurrentEmpresaId);
        }
    }
}