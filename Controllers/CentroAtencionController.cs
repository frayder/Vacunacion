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
    public class CentroAtencionController : BaseAuthorizationController
    {
        private readonly ApplicationDbContext _context;
        private readonly IImportExportService _importExportService;
        private readonly IEntityConfigurationService _configurationService;
        private readonly IDataPersistenceService _persistenceService;

        public CentroAtencionController(
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

        // GET: CentroAtencion
        public async Task<IActionResult> Index()
        {
            try
            { 

                // Validar permisos y obtener todos los permisos del módulo
                var (redirect, permissions) = await ValidateAndGetPermissionsAsync("CentrosAtencion", "Read");
                if (redirect != null) return redirect;

                var centrosAtencion = await _context.CentrosAtencion
                    .Where(c => c.EmpresaId == CurrentEmpresaId)
                    .OrderBy(c => c.Codigo)
                    .Select(c => new CentroAtencionItemViewModel
                    {
                        Id = c.Id,
                        Codigo = c.Codigo,
                        Nombre = c.Nombre,
                        Tipo = c.Tipo,
                        Estado = c.Estado,
                        Descripcion = c.Descripcion,
                        FechaCreacion = c.FechaCreacion
                    })
                    .ToListAsync();

                var viewModel = new CentroAtencionViewModel
                {
                    TotalCentros = centrosAtencion.Count,
                    CentrosActivos = centrosAtencion.Count(c => c.Estado),
                    CentrosInactivos = centrosAtencion.Count(c => !c.Estado),
                    CentrosAtencion = centrosAtencion,
                    // Agregar permisos al ViewModel
                    CanCreate = permissions["Create"],
                    CanUpdate = permissions["Update"],
                    CanDelete = permissions["Delete"]
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los centros de atención: " + ex.Message;
                return View(new CentroAtencionViewModel());
            }
        }

        // GET: CentroAtencion/Exportar
        public async Task<IActionResult> Exportar()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasExportPermission = await _authorizationService.HasPermissionAsync(userId, "CentrosAtencion", "Export") || 
                                    await _authorizationService.HasPermissionAsync(userId, "CentrosAtencion", "Read");
            
            if (!hasExportPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para exportar centros de atención.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var centros = await _context.CentrosAtencion
                    .Where(c => c.EmpresaId == CurrentEmpresaId)
                    .OrderBy(c => c.Codigo)
                    .ToListAsync();

                var exportConfig = _configurationService.GetExportConfiguration<CentroAtencion>();
                var excelData = await _importExportService.ExportToExcelAsync(centros, exportConfig);
                var fileName = $"{exportConfig.FileName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al exportar los centros de atención: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: CentroAtencion/ImportarPlantilla
        public async Task<IActionResult> ImportarPlantilla()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasImportPermission = await _authorizationService.HasPermissionAsync(userId, "CentrosAtencion", "Import") || 
                                    await _authorizationService.HasPermissionAsync(userId, "CentrosAtencion", "Create");
            
            if (!hasImportPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para importar centros de atención.";
                return RedirectToAction(nameof(Index));
            }

            return View(new ImportarCentroAtencionViewModel());
        }

        // GET: CentroAtencion/DescargarPlantilla
        [HttpGet]
        [ActionName("DescargarPlantilla")]
        public IActionResult DescargarPlantilla()
        {
            try
            {
                var importConfig = _configurationService.GetImportConfiguration<CentroAtencionItemViewModel>();
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

        // POST: CentroAtencion/ImportarPlantilla
        [HttpPost]
        [ActionName("ImportarPlantilla")]
        public async Task<IActionResult> ImportarPlantilla(ImportarCentroAtencionViewModel model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasImportPermission = await _authorizationService.HasPermissionAsync(userId, "CentrosAtencion", "Import") || 
                                    await _authorizationService.HasPermissionAsync(userId, "CentrosAtencion", "Create");
            
            if (!hasImportPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para importar centros de atención.";
                return RedirectToAction(nameof(Index));
            }

            if (model.ArchivoExcel == null || model.ArchivoExcel.Length == 0)
            {
                ModelState.AddModelError("ArchivoExcel", "Debe seleccionar un archivo Excel.");
                return View(model);
            }

            try
            {
                var importConfig = _configurationService.GetImportConfiguration<CentroAtencionItemViewModel>();
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

                HttpContext.Session.SetString("CentrosCargados", JsonConvert.SerializeObject(importResult.Data));
                model.CentrosCargados = importResult.Data;
                TempData["Success"] = $"Se procesaron {importResult.Data.Count} centros de atención correctamente. Revise los datos y confirme la importación.";

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al procesar el archivo: " + ex.Message);
                return View(model);
            }
        }

        // POST: CentroAtencion/GuardarCentrosImportados
        [HttpPost]
        public async Task<IActionResult> GuardarCentrosImportados()
        {
            var json = HttpContext.Session.GetString("CentrosCargados");
            if (string.IsNullOrEmpty(json))
            {
                TempData["Error"] = "No hay datos para importar.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var centrosCargados = JsonConvert.DeserializeObject<List<CentroAtencionItemViewModel>>(json);
                if (centrosCargados == null || !centrosCargados.Any())
                {
                    TempData["Error"] = "No hay datos para importar.";
                    return RedirectToAction(nameof(Index));
                }

                var totalProcessed = await _persistenceService.SaveImportedDataAsync<CentroAtencion, CentroAtencionItemViewModel>(
                    centrosCargados,
                    CurrentEmpresaId,
                    // Create mapper
                    viewModel => new CentroAtencion
                    {
                        Codigo = viewModel.Codigo.ToUpper(),
                        Nombre = viewModel.Nombre,
                        Tipo = viewModel.Tipo,
                        Descripcion = viewModel.Descripcion,
                        Estado = viewModel.Estado,
                        FechaCreacion = DateTime.Now,
                        EmpresaId = CurrentEmpresaId
                    },
                    // Update mapper
                    (viewModel, existing) =>
                    {
                        existing.Nombre = viewModel.Nombre;
                        existing.Tipo = viewModel.Tipo;
                        existing.Descripcion = viewModel.Descripcion;
                        existing.Estado = viewModel.Estado;
                        return existing;
                    },
                    // Find existing
                    (viewModel, dbSet) => dbSet.FirstOrDefault(c => 
                        c.EmpresaId == CurrentEmpresaId && 
                        c.Codigo.ToUpper() == viewModel.Codigo.ToUpper())
                );

                HttpContext.Session.Remove("CentrosCargados");
                TempData["Success"] = $"Importación completada: {totalProcessed} centros de atención procesados exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al guardar los centros de atención: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: CentroAtencion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var centroAtencion = await _context.CentrosAtencion
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

            if (centroAtencion == null)
            {
                return NotFound();
            }

            var viewModel = new CentroAtencionItemViewModel
            {
                Id = centroAtencion.Id,
                Codigo = centroAtencion.Codigo,
                Nombre = centroAtencion.Nombre,
                Tipo = centroAtencion.Tipo,
                Estado = centroAtencion.Estado,
                Descripcion = centroAtencion.Descripcion,
                FechaCreacion = centroAtencion.FechaCreacion
            };

            return View(viewModel);
        }

        // GET: CentroAtencion/Create
        public IActionResult Create()
        {
            return View(new CentroAtencionCreateViewModel());
        }

        // POST: CentroAtencion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CentroAtencionCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el código ya existe
                    var existeCodigo = await _context.CentrosAtencion
                        .AnyAsync(c => c.Codigo == viewModel.Codigo && c.EmpresaId == CurrentEmpresaId);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe un centro de atención con este código.");
                        return View(viewModel);
                    }

                    var centroAtencion = new CentroAtencion
                    {
                        Codigo = viewModel.Codigo,
                        Nombre = viewModel.Nombre,
                        Tipo = viewModel.Tipo,
                        Estado = viewModel.Estado,
                        Descripcion = viewModel.Descripcion,
                        FechaCreacion = DateTime.Now,
                        EmpresaId = CurrentEmpresaId
                    };

                    _context.Add(centroAtencion);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Centro de atención creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al crear el centro de atención: " + ex.Message;
                }
            }

            return View(viewModel);
        }

        // GET: CentroAtencion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var centroAtencion = await _context.CentrosAtencion
                .FirstOrDefaultAsync(c => c.Id == id && c.EmpresaId == CurrentEmpresaId);
            if (centroAtencion == null)
            {
                return NotFound();
            }

            var viewModel = new CentroAtencionEditViewModel
            {
                Id = centroAtencion.Id,
                Codigo = centroAtencion.Codigo,
                Nombre = centroAtencion.Nombre,
                Tipo = centroAtencion.Tipo,
                Estado = centroAtencion.Estado,
                FechaCreacion = centroAtencion.FechaCreacion
            };

            return View(viewModel);
        }

        // POST: CentroAtencion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CentroAtencionEditViewModel viewModel)
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
                    var existeCodigo = await _context.CentrosAtencion
                        .AnyAsync(c => c.Codigo == viewModel.Codigo && c.Id != id);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe un centro de atención con este código.");
                        return View(viewModel);
                    }

                    var centroAtencion = await _context.CentrosAtencion.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                    if (centroAtencion == null)
                    {
                        return NotFound();
                    }

                    centroAtencion.Codigo = viewModel.Codigo;
                    centroAtencion.Nombre = viewModel.Nombre;
                    centroAtencion.Tipo = viewModel.Tipo;
                    centroAtencion.Estado = viewModel.Estado;
                    centroAtencion.Descripcion = viewModel.Descripcion;

                    _context.Update(centroAtencion);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Centro de atención actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CentroAtencionExists(viewModel.Id))
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
                    TempData["Error"] = "Error al actualizar el centro de atención: " + ex.Message;
                }
            }

            return View(viewModel);
        }

        // GET: CentroAtencion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var centroAtencion = await _context.CentrosAtencion
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

            if (centroAtencion == null)
            {
                return NotFound();
            }

            var viewModel = new CentroAtencionItemViewModel
            {
                Id = centroAtencion.Id,
                Codigo = centroAtencion.Codigo,
                Nombre = centroAtencion.Nombre,
                Tipo = centroAtencion.Tipo,
                Estado = centroAtencion.Estado,
                FechaCreacion = centroAtencion.FechaCreacion,
                Descripcion = centroAtencion.Descripcion
            };

            return View(viewModel);
        }

        // POST: CentroAtencion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var centroAtencion = await _context.CentrosAtencion.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (centroAtencion != null)
                {
                    _context.CentrosAtencion.Remove(centroAtencion);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Centro de atención eliminado exitosamente.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el centro de atención: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: CentroAtencion/ToggleEstado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            try
            {
                var centroAtencion = await _context.CentrosAtencion.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (centroAtencion != null)
                {
                    centroAtencion.Estado = !centroAtencion.Estado;
                    _context.Update(centroAtencion);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, estado = centroAtencion.Estado });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = false, message = "Centro de atención no encontrado" });
        }

        private bool CentroAtencionExists(int id)
        {
            return _context.CentrosAtencion.Any(e => e.Id == id);
        }
    }
}