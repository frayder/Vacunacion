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
    public class CondicionUsuariaController : BaseAuthorizationController
    {
        private readonly ApplicationDbContext _context;
        private readonly IImportExportService _importExportService;
        private readonly IEntityConfigurationService _configurationService;
        private readonly IDataPersistenceService _persistenceService;

        public CondicionUsuariaController(
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

        // GET: CondicionUsuaria
        public async Task<IActionResult> Index()
        {
            try
            {
                var (redirect, permissions) = await ValidateAndGetPermissionsAsync("CondicionUsuaria", "Read");
                if (redirect != null) return redirect;

                var condiciones = await _context.CondicionesUsuarias
                    .Where(c => c.EmpresaId == CurrentEmpresaId)
                    .OrderBy(c => c.Codigo)
                    .Select(c => new CondicionUsuariaItemViewModel
                    {
                        Id = c.Id,
                        Codigo = c.Codigo,
                        Nombre = c.Nombre,
                        Descripcion = c.Descripcion,
                        Estado = c.Estado,
                        FechaCreacion = c.FechaCreacion
                    })
                    .ToListAsync();

                var viewModel = new CondicionUsuariaViewModel
                {
                    TotalCondiciones = condiciones.Count,
                    CondicionesActivas = condiciones.Count(c => c.Estado),
                    CondicionesInactivas = condiciones.Count(c => !c.Estado),
                    CondicionesUsuarias = condiciones,
                    CanCreate = permissions["Create"],
                    CanUpdate = permissions["Update"],
                    CanDelete = permissions["Delete"]
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar las condiciones de usuaria: " + ex.Message;
                return View(new CondicionUsuariaViewModel());
            }
        }

        // GET: CondicionUsuaria/Exportar
        public async Task<IActionResult> Exportar()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasExportPermission = await _authorizationService.HasPermissionAsync(userId, "CondicionUsuaria", "Export") || 
                                    await _authorizationService.HasPermissionAsync(userId, "CondicionUsuaria", "Read");
            
            if (!hasExportPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para exportar condiciones de usuaria.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var condiciones = await _context.CondicionesUsuarias
                    .Where(c => c.EmpresaId == CurrentEmpresaId)
                    .OrderBy(c => c.Codigo)
                    .ToListAsync();

                var exportConfig = _configurationService.GetExportConfiguration<CondicionUsuaria>();
                var excelData = await _importExportService.ExportToExcelAsync(condiciones, exportConfig);
                var fileName = $"{exportConfig.FileName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al exportar las condiciones de usuaria: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: CondicionUsuaria/ImportarPlantilla
        public async Task<IActionResult> ImportarPlantilla()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasImportPermission = await _authorizationService.HasPermissionAsync(userId, "CondicionUsuaria", "Import") || 
                                    await _authorizationService.HasPermissionAsync(userId, "CondicionUsuaria", "Create");
            
            if (!hasImportPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para importar condiciones de usuaria.";
                return RedirectToAction(nameof(Index));
            }

            return View(new ImportarCondicionUsuariaViewModel());
        }

        // GET: CondicionUsuaria/DescargarPlantilla
        [HttpGet]
        [ActionName("DescargarPlantilla")]
        public IActionResult DescargarPlantilla()
        {
            try
            {
                var importConfig = _configurationService.GetImportConfiguration<CondicionUsuariaItemViewModel>();
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

        // POST: CondicionUsuaria/ImportarPlantilla
        [HttpPost]
        [ActionName("ImportarPlantilla")]
        public async Task<IActionResult> ImportarPlantilla(ImportarCondicionUsuariaViewModel model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasImportPermission = await _authorizationService.HasPermissionAsync(userId, "CondicionUsuaria", "Import") || 
                                    await _authorizationService.HasPermissionAsync(userId, "CondicionUsuaria", "Create");
            
            if (!hasImportPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para importar condiciones de usuaria.";
                return RedirectToAction(nameof(Index));
            }

            if (model.ArchivoExcel == null || model.ArchivoExcel.Length == 0)
            {
                ModelState.AddModelError("ArchivoExcel", "Debe seleccionar un archivo Excel.");
                return View(model);
            }

            try
            {
                var importConfig = _configurationService.GetImportConfiguration<CondicionUsuariaItemViewModel>();
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

                HttpContext.Session.SetString("CondicionesCargadas", JsonConvert.SerializeObject(importResult.Data));
                model.CondicionesCargadas = importResult.Data;
                TempData["Success"] = $"Se procesaron {importResult.Data.Count} condiciones correctamente. Revise los datos y confirme la importación.";

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al procesar el archivo: " + ex.Message);
                return View(model);
            }
        }

        // POST: CondicionUsuaria/GuardarCondicionesImportadas
        [HttpPost]
        public async Task<IActionResult> GuardarCondicionesImportadas()
        {
            var json = HttpContext.Session.GetString("CondicionesCargadas");
            if (string.IsNullOrEmpty(json))
            {
                TempData["Error"] = "No hay datos para importar.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var condicionesCargadas = JsonConvert.DeserializeObject<List<CondicionUsuariaItemViewModel>>(json);
                if (condicionesCargadas == null || !condicionesCargadas.Any())
                {
                    TempData["Error"] = "No hay datos para importar.";
                    return RedirectToAction(nameof(Index));
                }

                var totalProcessed = await _persistenceService.SaveImportedDataAsync<CondicionUsuaria, CondicionUsuariaItemViewModel>(
                    condicionesCargadas,
                    CurrentEmpresaId,
                    // Create mapper
                    viewModel => new CondicionUsuaria
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
                    (viewModel, dbSet) => dbSet.FirstOrDefault(c => 
                        c.EmpresaId == CurrentEmpresaId && 
                        c.Codigo.ToUpper() == viewModel.Codigo.ToUpper())
                );

                HttpContext.Session.Remove("CondicionesCargadas");
                TempData["Success"] = $"Importación completada: {totalProcessed} condiciones procesadas exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al guardar las condiciones: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: CondicionUsuaria/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var condicionUsuaria = await _context.CondicionesUsuarias
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

            if (condicionUsuaria == null)
            {
                return NotFound();
            }

            var viewModel = new CondicionUsuariaItemViewModel
            {
                Id = condicionUsuaria.Id,
                Codigo = condicionUsuaria.Codigo,
                Nombre = condicionUsuaria.Nombre,
                Descripcion = condicionUsuaria.Descripcion,
                Estado = condicionUsuaria.Estado,
                FechaCreacion = condicionUsuaria.FechaCreacion
            };

            return View(viewModel);
        }

        // GET: CondicionUsuaria/Create
        public IActionResult Create()
        {
            return View(new CondicionUsuariaCreateViewModel());
        }

        // POST: CondicionUsuaria/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CondicionUsuariaCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Validar permiso de creación
                    var redirect = await ValidatePermissionAsync("CondicionUsuaria", "Create");
                    if (redirect != null) return redirect;

                    // Verificar si el código ya existe
                    var existeCodigo = await _context.CondicionesUsuarias
                        .AnyAsync(c => c.Codigo == viewModel.Codigo);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe una condición usuaria con este código.");
                        return View(viewModel);
                    }

                    var condicionUsuaria = new CondicionUsuaria
                    {
                        Codigo = viewModel.Codigo,
                        Nombre = viewModel.Nombre,
                        Descripcion = viewModel.Descripcion,
                        Estado = viewModel.Estado,
                        FechaCreacion = DateTime.Now,
                        EmpresaId = CurrentEmpresaId
                    };

                    _context.Add(condicionUsuaria);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Condición usuaria creada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al crear la condición usuaria: " + ex.Message;
                }
            }

            return View(viewModel);
        }

        // GET: CondicionUsuaria/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            // Validar permiso de actualización
            var redirect = await ValidatePermissionAsync("CondicionUsuaria", "Update");
            if (redirect != null) return redirect;

            if (id == null)
            {
                return NotFound();
            }

            var condicionUsuaria = await _context.CondicionesUsuarias.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
            if (condicionUsuaria == null)
            {
                return NotFound();
            }

            var viewModel = new CondicionUsuariaEditViewModel
            {
                Id = condicionUsuaria.Id,
                Codigo = condicionUsuaria.Codigo,
                Nombre = condicionUsuaria.Nombre,
                Descripcion = condicionUsuaria.Descripcion,
                Estado = condicionUsuaria.Estado,
                FechaCreacion = condicionUsuaria.FechaCreacion
            };

            return View(viewModel);
        }

        // POST: CondicionUsuaria/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CondicionUsuariaEditViewModel viewModel)
        {
            // Validar permiso de actualización
            var redirect = await ValidatePermissionAsync("CondicionUsuaria", "Update");
            if (redirect != null) return redirect;

            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el código ya existe (excluyendo el registro actual)
                    var existeCodigo = await _context.CondicionesUsuarias
                        .AnyAsync(c => c.Codigo == viewModel.Codigo && c.Id != id);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe una condición usuaria con este código.");
                        return View(viewModel);
                    }

                    var condicionUsuaria = await _context.CondicionesUsuarias.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                    if (condicionUsuaria == null)
                    {
                        return NotFound();
                    }

                    condicionUsuaria.Codigo = viewModel.Codigo;
                    condicionUsuaria.Nombre = viewModel.Nombre;
                    condicionUsuaria.Descripcion = viewModel.Descripcion;
                    condicionUsuaria.Estado = viewModel.Estado;

                    _context.Update(condicionUsuaria);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Condición usuaria actualizada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CondicionUsuariaExists(viewModel.Id))
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
                    TempData["Error"] = "Error al actualizar la condición usuaria: " + ex.Message;
                }
            }

            return View(viewModel);
        }

        // GET: CondicionUsuaria/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
             // Validar permiso de eliminación
            var redirect = await ValidatePermissionAsync("CondicionUsuaria", "Delete");
            if (redirect != null) return redirect;

            if (id == null)
            {
                return NotFound();
            }

            var condicionUsuaria = await _context.CondicionesUsuarias
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

            if (condicionUsuaria == null)
            {
                return NotFound();
            }

            var viewModel = new CondicionUsuariaItemViewModel
            {
                Id = condicionUsuaria.Id,
                Codigo = condicionUsuaria.Codigo,
                Nombre = condicionUsuaria.Nombre,
                Descripcion = condicionUsuaria.Descripcion,
                Estado = condicionUsuaria.Estado,
                FechaCreacion = condicionUsuaria.FechaCreacion
            };

            return View(viewModel);
        }

        // POST: CondicionUsuaria/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                 // Validar permiso de eliminación
                var redirect = await ValidatePermissionAsync("CondicionUsuaria", "Delete");
                if (redirect != null) return redirect;

                var condicionUsuaria = await _context.CondicionesUsuarias.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (condicionUsuaria != null)
                {
                    _context.CondicionesUsuarias.Remove(condicionUsuaria);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Condición usuaria eliminada exitosamente.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar la condición usuaria: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: CondicionUsuaria/ToggleEstado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            try
            {
                var condicionUsuaria = await _context.CondicionesUsuarias.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (condicionUsuaria != null)
                {
                    condicionUsuaria.Estado = !condicionUsuaria.Estado;
                    _context.Update(condicionUsuaria);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, estado = condicionUsuaria.Estado });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = false, message = "Condición usuaria no encontrada" });
        }

        private bool CondicionUsuariaExists(int id)
        {
            return _context.CondicionesUsuarias.Any(e => e.Id == id);
        }
    }
}