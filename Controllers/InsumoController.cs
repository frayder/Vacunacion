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
    public class InsumoController : BaseAuthorizationController
    {
        private readonly ApplicationDbContext _context;
        private readonly IImportExportService _importExportService;
        private readonly IEntityConfigurationService _configurationService;
        private readonly IDataPersistenceService _persistenceService;

        public InsumoController(
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

        // GET: Insumo
        public async Task<IActionResult> Index()
        {
            try
            {
                var (redirect, permissions) = await ValidateAndGetPermissionsAsync("Insumos", "Read");
                if (redirect != null) return redirect;

                var insumos = await _context.Insumos
                    .Where(i => i.EmpresaId == CurrentEmpresaId)
                    .OrderBy(i => i.Codigo)
                    .Select(i => new InsumoItemViewModel
                    {
                        Id = i.Id,
                        Codigo = i.Codigo,
                        Nombre = i.Nombre,
                        Descripcion = i.Descripcion,
                        Estado = i.Estado,
                        FechaCreacion = i.FechaCreacion
                    })
                    .ToListAsync();

                var viewModel = new InsumoViewModel
                {
                    TotalInsumos = insumos.Count,
                    InsumosActivos = insumos.Count(i => i.Estado),
                    InsumosInactivos = insumos.Count(i => !i.Estado),
                    Insumos = insumos,
                    CanCreate = permissions["Create"],
                    CanUpdate = permissions["Update"],
                    CanDelete = permissions["Delete"]
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los insumos: " + ex.Message;
                return View(new InsumoViewModel());
            }
        }

        // GET: Insumo/Exportar
        public async Task<IActionResult> Exportar()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasExportPermission = await _authorizationService.HasPermissionAsync(userId, "Insumo", "Export") || 
                                    await _authorizationService.HasPermissionAsync(userId, "Insumo", "Read");
            
            if (!hasExportPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para exportar insumos.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var insumos = await _context.Insumos
                    .Where(i => i.EmpresaId == CurrentEmpresaId)
                    .OrderBy(i => i.Codigo)
                    .ToListAsync();

                var exportConfig = _configurationService.GetExportConfiguration<Insumo>();
                var excelData = await _importExportService.ExportToExcelAsync(insumos, exportConfig);
                var fileName = $"{exportConfig.FileName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al exportar los insumos: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Insumo/ImportarPlantilla
        public async Task<IActionResult> ImportarPlantilla()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasImportPermission = await _authorizationService.HasPermissionAsync(userId, "Insumo", "Import") || 
                                    await _authorizationService.HasPermissionAsync(userId, "Insumo", "Create");
            
            if (!hasImportPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para importar insumos.";
                return RedirectToAction(nameof(Index));
            }

            return View(new ImportarInsumoViewModel());
        }

        // GET: Insumo/DescargarPlantilla
        [HttpGet]
        [ActionName("DescargarPlantilla")]
        public IActionResult DescargarPlantilla()
        {
            try
            {
                var importConfig = _configurationService.GetImportConfiguration<InsumoItemViewModel>();
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

        // POST: Insumo/ImportarPlantilla
        [HttpPost]
        [ActionName("ImportarPlantilla")]
        public async Task<IActionResult> ImportarPlantilla(ImportarInsumoViewModel model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasImportPermission = await _authorizationService.HasPermissionAsync(userId, "Insumo", "Import") || 
                                    await _authorizationService.HasPermissionAsync(userId, "Insumo", "Create");
            
            if (!hasImportPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para importar insumos.";
                return RedirectToAction(nameof(Index));
            }

            if (model.ArchivoExcel == null || model.ArchivoExcel.Length == 0)
            {
                ModelState.AddModelError("ArchivoExcel", "Debe seleccionar un archivo Excel.");
                return View(model);
            }

            try
            {
                var importConfig = _configurationService.GetImportConfiguration<InsumoItemViewModel>();
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

                HttpContext.Session.SetString("InsumosCargados", JsonConvert.SerializeObject(importResult.Data));
                model.InsumosCargados = importResult.Data;
                TempData["Success"] = $"Se procesaron {importResult.Data.Count} insumos correctamente. Revise los datos y confirme la importación.";

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al procesar el archivo: " + ex.Message);
                return View(model);
            }
        }

        // POST: Insumo/GuardarInsumosImportados
        [HttpPost]
        public async Task<IActionResult> GuardarInsumosImportados()
        {
            var json = HttpContext.Session.GetString("InsumosCargados");
            if (string.IsNullOrEmpty(json))
            {
                TempData["Error"] = "No hay datos para importar.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var insumosCargados = JsonConvert.DeserializeObject<List<InsumoItemViewModel>>(json);
                if (insumosCargados == null || !insumosCargados.Any())
                {
                    TempData["Error"] = "No hay datos para importar.";
                    return RedirectToAction(nameof(Index));
                }

                var totalProcessed = await _persistenceService.SaveImportedDataAsync<Insumo, InsumoItemViewModel>(
                    insumosCargados,
                    CurrentEmpresaId,
                    // Create mapper
                    viewModel => new Insumo
                    {
                        Codigo = viewModel.Codigo.ToUpper(),
                        Nombre = viewModel.Nombre,
                        Descripcion = viewModel.Descripcion,
                        Estado = viewModel.Estado,
                        FechaCreacion = DateTime.Now,
                        EmpresaId = CurrentEmpresaId,
                        EdadMinima = viewModel.EdadMinima,
                        EdadMaxima = viewModel.EdadMaxima,
                        UnidadMedidaEdadMinima = viewModel.UnidadMedidaEdadMinima,
                        UnidadMedidaEdadMaxima = viewModel.UnidadMedidaEdadMaxima
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
                    (viewModel, dbSet) => dbSet.FirstOrDefault(i => 
                        i.EmpresaId == CurrentEmpresaId && 
                        i.Codigo.ToUpper() == viewModel.Codigo.ToUpper())
                );

                HttpContext.Session.Remove("InsumosCargados");
                TempData["Success"] = $"Importación completada: {totalProcessed} insumos procesados exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al guardar los insumos: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Insumo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insumo = await _context.Insumos
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

            if (insumo == null)
            {
                return NotFound();
            }

            var viewModel = new InsumoItemViewModel
            {
                Id = insumo.Id,
                Codigo = insumo.Codigo,
                Nombre = insumo.Nombre,
                Tipo = insumo.Tipo,
                Descripcion = insumo.Descripcion,
                RangoDosis = insumo.RangoDosis,
                Estado = insumo.Estado,
                FechaCreacion = insumo.FechaCreacion,
                EdadMinima = insumo.EdadMinima,
                EdadMaxima = insumo.EdadMaxima,
                UnidadMedidaEdadMinima = insumo.UnidadMedidaEdadMinima,
                UnidadMedidaEdadMaxima = insumo.UnidadMedidaEdadMaxima
            };

            return View(viewModel);
        }

        // GET: Insumo/Create
        public IActionResult Create()
        {
            return View(new InsumoCreateViewModel());
        }

        // POST: Insumo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InsumoCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el código ya existe
                    var existeCodigo = await _context.Insumos
                        .AnyAsync(i => i.Codigo == viewModel.Codigo && i.EmpresaId == CurrentEmpresaId);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe un insumo con este código.");
                        return View(viewModel);
                    }

                    // Construir rango/dosis si se proporcionaron datos
                    string? rangoDosis =  viewModel.EdadMinima + " " + viewModel.UnidadMedidaEdadMinima + " - " + viewModel.EdadMaxima + " " + viewModel.UnidadMedidaEdadMaxima;
                 

                    var insumo = new Insumo
                    {
                        Codigo = viewModel.Codigo,
                        Nombre = viewModel.Nombre,
                        Tipo = viewModel.Tipo,
                        Descripcion = viewModel.Descripcion,
                        RangoDosis = rangoDosis,
                        Estado = viewModel.Estado,
                        FechaCreacion = DateTime.Now,
                        EmpresaId = CurrentEmpresaId,
                        EdadMinima = viewModel.EdadMinima,
                        EdadMaxima = viewModel.EdadMaxima,
                        UnidadMedidaEdadMinima = viewModel.UnidadMedidaEdadMinima,
                        UnidadMedidaEdadMaxima = viewModel.UnidadMedidaEdadMaxima
                    };

                    _context.Add(insumo);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Insumo creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear el insumo: " + ex.Message);
                }
            }

            return View(viewModel);
        }

        // GET: Insumo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insumo = await _context.Insumos.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
            if (insumo == null)
            {
                return NotFound();
            }

            var viewModel = new InsumoEditViewModel
            {
                Id = insumo.Id,
                Codigo = insumo.Codigo,
                Nombre = insumo.Nombre,
                Tipo = insumo.Tipo,
                Descripcion = insumo.Descripcion,
                RangoDosis = insumo.RangoDosis,
                Estado = insumo.Estado,
                FechaCreacion = insumo.FechaCreacion,
                EdadMinima = insumo.EdadMinima,
                EdadMaxima = insumo.EdadMaxima,
                UnidadMedidaEdadMinima = insumo.UnidadMedidaEdadMinima,
                UnidadMedidaEdadMaxima = insumo.UnidadMedidaEdadMaxima
            };

            return View(viewModel);
        }

        // POST: Insumo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InsumoEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el código ya existe en otro registro
                    var existeCodigo = await _context.Insumos
                        .AnyAsync(i => i.Codigo == viewModel.Codigo && i.Id != id && i.EmpresaId == CurrentEmpresaId);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe otro insumo con este código.");
                        return View(viewModel);
                    }

                    var insumo = await _context.Insumos.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                    if (insumo == null)
                    {
                        return NotFound();
                    }

                    insumo.Codigo = viewModel.Codigo;
                    insumo.Nombre = viewModel.Nombre;
                    insumo.Tipo = viewModel.Tipo;
                    insumo.Descripcion = viewModel.Descripcion;
                    insumo.RangoDosis = viewModel.RangoDosis;
                    insumo.Estado = viewModel.Estado;
                    insumo.EdadMinima = viewModel.EdadMinima;
                    insumo.EdadMaxima = viewModel.EdadMaxima;
                    insumo.UnidadMedidaEdadMinima = viewModel.UnidadMedidaEdadMinima;
                    insumo.UnidadMedidaEdadMaxima = viewModel.UnidadMedidaEdadMaxima;

                    _context.Update(insumo);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Insumo actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InsumoExists(viewModel.Id))
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
                    ModelState.AddModelError("", "Error al actualizar el insumo: " + ex.Message);
                }
            }

            return View(viewModel);
        }

        // GET: Insumo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insumo = await _context.Insumos
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

            if (insumo == null)
            {
                return NotFound();
            }

            var viewModel = new InsumoItemViewModel
            {
                Id = insumo.Id,
                Codigo = insumo.Codigo,
                Nombre = insumo.Nombre,
                Tipo = insumo.Tipo,
                Descripcion = insumo.Descripcion,
                RangoDosis = insumo.RangoDosis,
                Estado = insumo.Estado,
                FechaCreacion = insumo.FechaCreacion,
                EdadMinima = insumo.EdadMinima,
                EdadMaxima = insumo.EdadMaxima,
                UnidadMedidaEdadMinima = insumo.UnidadMedidaEdadMinima,
                UnidadMedidaEdadMaxima = insumo.UnidadMedidaEdadMaxima
            };

            return View(viewModel);
        }

        // POST: Insumo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var insumo = await _context.Insumos.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (insumo != null)
                {
                    _context.Insumos.Remove(insumo);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Insumo eliminado exitosamente.";
                }
                else
                {
                    TempData["Error"] = "No se encontró el insumo a eliminar.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el insumo: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool InsumoExists(int id)
        {
            return _context.Insumos.Any(e => e.Id == id);
        }

        // AJAX Methods for modal operations
        [HttpGet]
        public async Task<JsonResult> GetById(int id)
        {
            try
            {
                var insumo = await _context.Insumos.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (insumo == null)
                {
                    return Json(new { success = false, message = "Insumo no encontrado" });
                }

                var viewModel = new InsumoItemViewModel
                {
                    Id = insumo.Id,
                    Codigo = insumo.Codigo,
                    Nombre = insumo.Nombre,
                    Tipo = insumo.Tipo,
                    Descripcion = insumo.Descripcion,
                    RangoDosis = insumo.RangoDosis,
                    Estado = insumo.Estado,
                    FechaCreacion = insumo.FechaCreacion,
                    EdadMinima = insumo.EdadMinima,
                    EdadMaxima = insumo.EdadMaxima,
                    UnidadMedidaEdadMinima = insumo.UnidadMedidaEdadMinima,
                    UnidadMedidaEdadMaxima = insumo.UnidadMedidaEdadMaxima
                };

                return Json(new { success = true, data = viewModel });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> CreateAjax([FromBody] InsumoCreateViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { Field = x.Key, Message = x.Value.Errors.First().ErrorMessage })
                        .ToArray();
                    return Json(new { success = false, message = "Datos inválidos", errors });
                }

                // Verificar si el código ya existe
                var existeCodigo = await _context.Insumos
                    .AnyAsync(i => i.Codigo == viewModel.Codigo && i.EmpresaId == CurrentEmpresaId);

                if (existeCodigo)
                {
                    return Json(new { success = false, message = "Ya existe un insumo con este código." });
                }

                var insumo = new Insumo
                {
                    Codigo = viewModel.Codigo,
                    Nombre = viewModel.Nombre,
                    Tipo = viewModel.Tipo,
                    Descripcion = viewModel.Descripcion,
                    Estado = viewModel.Estado,
                    FechaCreacion = DateTime.Now,
                    EdadMinima = viewModel.EdadMinima,
                    EdadMaxima = viewModel.EdadMaxima,
                    UnidadMedidaEdadMinima = viewModel.UnidadMedidaEdadMinima,
                    UnidadMedidaEdadMaxima = viewModel.UnidadMedidaEdadMaxima
                };

                _context.Add(insumo);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Insumo creado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> EditAjax([FromBody] InsumoEditViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { Field = x.Key, Message = x.Value.Errors.First().ErrorMessage })
                        .ToArray();
                    return Json(new { success = false, message = "Datos inválidos", errors });
                }

                // Verificar si el código ya existe en otro registro
                var existeCodigo = await _context.Insumos
                    .AnyAsync(i => i.Codigo == viewModel.Codigo && i.Id != viewModel.Id && i.EmpresaId == CurrentEmpresaId);

                if (existeCodigo)
                {
                    return Json(new { success = false, message = "Ya existe otro insumo con este código." });
                }

                var insumo = await _context.Insumos.FindAsync(viewModel.Id);
                if (insumo == null)
                {
                    return Json(new { success = false, message = "Insumo no encontrado" });
                }

                insumo.Codigo = viewModel.Codigo;
                insumo.Nombre = viewModel.Nombre;
                insumo.Tipo = viewModel.Tipo;
                insumo.Descripcion = viewModel.Descripcion;
                insumo.RangoDosis = viewModel.RangoDosis;
                insumo.Estado = viewModel.Estado;
                insumo.EdadMinima = viewModel.EdadMinima;
                insumo.EdadMaxima = viewModel.EdadMaxima;
                insumo.UnidadMedidaEdadMinima = viewModel.UnidadMedidaEdadMinima;
                insumo.UnidadMedidaEdadMaxima = viewModel.UnidadMedidaEdadMaxima;

                _context.Update(insumo);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Insumo actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteAjax(int id)
        {
            try
            {
                var insumo = await _context.Insumos.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (insumo == null)
                {
                    return Json(new { success = false, message = "Insumo no encontrado" });
                }

                _context.Insumos.Remove(insumo);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Insumo eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
    }
}