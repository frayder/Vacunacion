using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Highdmin.Data;
using Highdmin.Models;
using Highdmin.ViewModels;
using Highdmin.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Newtonsoft.Json;

namespace Highdmin.Controllers
{
    [Authorize]
    public class RegimenAfiliacionController : BaseAuthorizationController
    {
        private readonly ApplicationDbContext _context;
        private readonly IImportExportService _importExportService;
        private readonly IEntityConfigurationService _configurationService;
        private readonly IDataPersistenceService _persistenceService;

        public RegimenAfiliacionController(
            ApplicationDbContext context, 
            IEmpresaService empresaService, 
            AuthorizationService authorizationService,
            IImportExportService importExportService,
            IEntityConfigurationService configurationService,
            IDataPersistenceService persistenceService
            ) 
            : base(empresaService, authorizationService)
        {
            _context = context;
            _importExportService = importExportService;
            _configurationService = configurationService;
            _persistenceService = persistenceService;
        }

        // GET: RegimenAfiliacion
        public async Task<IActionResult> Index()
        {
            try
            {
                var (redirect, permissions) = await ValidateAndGetPermissionsAsync("RegimenAfiliacion", "Read");
                if (redirect != null) return redirect;

                var regimenesAfiliacion = await _context.RegimenesAfiliacion
                    .Where(r => r.EmpresaId == CurrentEmpresaId)
                    .OrderBy(r => r.Codigo)
                    .Select(r => new RegimenAfiliacionItemViewModel
                    {
                        Id = r.Id,
                        Codigo = r.Codigo,
                        Nombre = r.Nombre,
                        Descripcion = r.Descripcion,
                        Estado = r.Estado,
                        FechaCreacion = r.FechaCreacion
                    })
                    .ToListAsync();

                var viewModel = new RegimenAfiliacionViewModel
                {
                    TotalRegimenes = regimenesAfiliacion.Count,
                    RegimenesActivos = regimenesAfiliacion.Count(r => r.Estado),
                    RegimenesInactivos = regimenesAfiliacion.Count(r => !r.Estado),
                    RegimenesAfiliacion = regimenesAfiliacion,
                    // Agregar permisos al ViewModel
                    CanCreate = permissions["Create"],
                    CanUpdate = permissions["Update"],
                    CanDelete = permissions["Delete"]
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los regímenes de afiliación: " + ex.Message;
                return View(new RegimenAfiliacionViewModel());
            }
        }

        // GET: RegimenAfiliacion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var regimenAfiliacion = await _context.RegimenesAfiliacion
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

            if (regimenAfiliacion == null)
            {
                return NotFound();
            }

            var viewModel = new RegimenAfiliacionItemViewModel
            {
                Id = regimenAfiliacion.Id,
                Codigo = regimenAfiliacion.Codigo,
                Nombre = regimenAfiliacion.Nombre,
                Descripcion = regimenAfiliacion.Descripcion,
                Estado = regimenAfiliacion.Estado,
                FechaCreacion = regimenAfiliacion.FechaCreacion
            };

            return View(viewModel);
        }

        // GET: RegimenAfiliacion/Create
        public IActionResult Create()
        {
            return View(new RegimenAfiliacionCreateViewModel());
        }

        // POST: RegimenAfiliacion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegimenAfiliacionCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el código ya existe
                    var existeCodigo = await _context.RegimenesAfiliacion
                        .AnyAsync(r => r.Codigo == viewModel.Codigo && r.EmpresaId == CurrentEmpresaId);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe un régimen de afiliación con este código.");
                        return View(viewModel);
                    }

                    var regimenAfiliacion = new RegimenAfiliacion
                    {
                        Codigo = viewModel.Codigo,
                        Nombre = viewModel.Nombre,
                        Descripcion = viewModel.Descripcion,
                        Estado = viewModel.Estado,
                        FechaCreacion = DateTime.Now,
                        EmpresaId = CurrentEmpresaId
                    };

                    _context.Add(regimenAfiliacion);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Régimen de afiliación creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al crear el régimen de afiliación: " + ex.Message;
                }
            }

            return View(viewModel);
        }

        // GET: RegimenAfiliacion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var regimenAfiliacion = await _context.RegimenesAfiliacion
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
            if (regimenAfiliacion == null)
            {
                return NotFound();
            }

            var viewModel = new RegimenAfiliacionEditViewModel
            {
                Id = regimenAfiliacion.Id,
                Codigo = regimenAfiliacion.Codigo,
                Nombre = regimenAfiliacion.Nombre,
                Descripcion = regimenAfiliacion.Descripcion,
                Estado = regimenAfiliacion.Estado,
                FechaCreacion = regimenAfiliacion.FechaCreacion
            };

            return View(viewModel);
        }

        // POST: RegimenAfiliacion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RegimenAfiliacionEditViewModel viewModel)
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
                    var existeCodigo = await _context.RegimenesAfiliacion
                        .AnyAsync(r => r.Codigo == viewModel.Codigo && r.Id != id);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe un régimen de afiliación con este código.");
                        return View(viewModel);
                    }

                    var regimenAfiliacion = await _context.RegimenesAfiliacion
                        .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                    if (regimenAfiliacion == null)
                    {
                        return NotFound();
                    }

                    regimenAfiliacion.Codigo = viewModel.Codigo;
                    regimenAfiliacion.Nombre = viewModel.Nombre;
                    regimenAfiliacion.Descripcion = viewModel.Descripcion;
                    regimenAfiliacion.Estado = viewModel.Estado;

                    _context.Update(regimenAfiliacion);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Régimen de afiliación actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegimenAfiliacionExists(viewModel.Id))
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
                    TempData["Error"] = "Error al actualizar el régimen de afiliación: " + ex.Message;
                }
            }

            return View(viewModel);
        }

        // GET: RegimenAfiliacion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var regimenAfiliacion = await _context.RegimenesAfiliacion
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

            if (regimenAfiliacion == null)
            {
                return NotFound();
            }

            var viewModel = new RegimenAfiliacionItemViewModel
            {
                Id = regimenAfiliacion.Id,
                Codigo = regimenAfiliacion.Codigo,
                Nombre = regimenAfiliacion.Nombre,
                Descripcion = regimenAfiliacion.Descripcion,
                Estado = regimenAfiliacion.Estado,
                FechaCreacion = regimenAfiliacion.FechaCreacion
            };

            return View(viewModel);
        }

        // POST: RegimenAfiliacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var regimenAfiliacion = await _context.RegimenesAfiliacion
                    .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (regimenAfiliacion != null)
                {
                    _context.RegimenesAfiliacion.Remove(regimenAfiliacion);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Régimen de afiliación eliminado exitosamente.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el régimen de afiliación: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: RegimenAfiliacion/ToggleEstado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            try
            {
                var regimenAfiliacion = await _context.RegimenesAfiliacion
                    .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (regimenAfiliacion != null)
                {
                    regimenAfiliacion.Estado = !regimenAfiliacion.Estado;
                    _context.Update(regimenAfiliacion);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, estado = regimenAfiliacion.Estado });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = false, message = "Régimen de afiliación no encontrado" });
        }

        // GET: TipoCarnet/Exportar
        public async Task<IActionResult> Exportar()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasExportPermission = await _authorizationService.HasPermissionAsync(userId, "RegimenAfiliacion", "Export") || 
                                    await _authorizationService.HasPermissionAsync(userId, "RegimenAfiliacion", "Read");

            if (!hasExportPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para exportar regímenes de afiliación.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var regimenesAfiliacion = await _context.RegimenesAfiliacion
                    .Where(t => t.EmpresaId == CurrentEmpresaId)
                    .OrderBy(t => t.Codigo)
                    .ToListAsync();

                var exportConfig = _configurationService.GetExportConfiguration<RegimenAfiliacion>();
                var excelData = await _importExportService.ExportToExcelAsync(regimenesAfiliacion, exportConfig);
                var fileName = $"{exportConfig.FileName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al exportar los tipos de carnet: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: RegimenAfiliacion/ImportarPlantilla
        public async Task<IActionResult> ImportarPlantilla()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasImportPermission = await _authorizationService.HasPermissionAsync(userId, "RegimenAfiliacion", "Import") ||
                                    await _authorizationService.HasPermissionAsync(userId, "RegimenAfiliacion", "Create");

            if (!hasImportPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para importar regímenes de afiliación.";
                return RedirectToAction(nameof(Index));
            }

            return View(new ImportarRegimenAfiliacionViewModel());
        }

        // GET: RegimenAfiliacion/DescargarPlantilla
        [HttpGet]
        [ActionName("DescargarPlantilla")]
        public IActionResult DescargarPlantilla()
        {
            try
            {
                var importConfig = _configurationService.GetImportConfiguration<RegimenAfiliacionItemViewModel>();
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

        // POST: RegimenAfiliacion/ImportarPlantilla
        [HttpPost]
        [ActionName("ImportarPlantilla")]
        public async Task<IActionResult> ImportarPlantilla(ImportarRegimenAfiliacionViewModel model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasImportPermission = await _authorizationService.HasPermissionAsync(userId, "RegimenAfiliacion", "Import") ||
                                    await _authorizationService.HasPermissionAsync(userId, "RegimenAfiliacion", "Create");

            if (!hasImportPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para importar regímenes de afiliación.";
                return RedirectToAction(nameof(Index));
            }

            if (model.ArchivoExcel == null || model.ArchivoExcel.Length == 0)
            {
                ModelState.AddModelError("ArchivoExcel", "Debe seleccionar un archivo Excel.");
                return View(model);
            }

            try
            {
                var importConfig = _configurationService.GetImportConfiguration<RegimenAfiliacionItemViewModel>();
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

                // Guardar en sesión para confirmar
                HttpContext.Session.SetString("RegimenesCargados", JsonConvert.SerializeObject(importResult.Data));
                model.RegimenesCargados = importResult.Data;
                TempData["Success"] = $"Se procesaron {importResult.Data.Count} regímenes correctamente. Revise los datos y confirme la importación.";

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al procesar el archivo: " + ex.Message);
                return View(model);
            }
        }

        // POST: RegimenAfiliacion/GuardarRegimenesImportados
        [HttpPost]
        public async Task<IActionResult> GuardarRegimenesImportados()
        {
            Console.WriteLine("Guardando regímenes importados...");
            var json = HttpContext.Session.GetString("RegimenesCargados");
            if (string.IsNullOrEmpty(json))
            {
                TempData["Error"] = "No hay datos para importar.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var regimenesCargados = JsonConvert.DeserializeObject<List<RegimenAfiliacionItemViewModel>>(json);
                if (regimenesCargados == null || !regimenesCargados.Any())
                {
                    TempData["Error"] = "No hay datos para importar.";
                    return RedirectToAction(nameof(Index));
                }

                var totalProcessed = await _persistenceService.SaveImportedDataAsync<RegimenAfiliacion, RegimenAfiliacionItemViewModel>(
                    regimenesCargados,
                    CurrentEmpresaId,
                    // Create mapper
                    viewModel => new RegimenAfiliacion
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
                    (viewModel, dbSet) => dbSet.FirstOrDefault(r =>
                        r.EmpresaId == CurrentEmpresaId &&
                        r.Codigo.ToUpper() == viewModel.Codigo.ToUpper())
                );

                HttpContext.Session.Remove("RegimenesCargados");
                TempData["Success"] = $"Importación completada: {totalProcessed} regímenes procesados exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al guardar los regímenes: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private bool RegimenAfiliacionExists(int id)
        {
            return _context.RegimenesAfiliacion.Any(e => e.Id == id && e.EmpresaId == CurrentEmpresaId);
        }
    }
}