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
    public class AseguradoraController : BaseAuthorizationController
    {
        private readonly ApplicationDbContext _context;
        private readonly IImportExportService _importExportService;
        private readonly IEntityConfigurationService _configurationService;
        private readonly IDataPersistenceService _persistenceService;

        public AseguradoraController(
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

        // GET: Aseguradora
        public async Task<IActionResult> Index()
        {
            try
            {
                // Validar permisos y obtener todos los permisos del módulo
                var (redirect, permissions) = await ValidateAndGetPermissionsAsync("Aseguradoras", "Read");
                if (redirect != null) return redirect;

                var aseguradoras = await _context.Aseguradoras
                    .Where(c => c.EmpresaId == CurrentEmpresaId)
                    .OrderBy(a => a.Codigo)
                    .Select(a => new AseguradoraItemViewModel
                    {
                        Id = a.Id,
                        Codigo = a.Codigo,
                        Nombre = a.Nombre,
                        Descripcion = a.Descripcion,
                        Estado = a.Estado,
                        FechaCreacion = a.FechaCreacion
                    })
                    .ToListAsync();

                var viewModel = new AseguradoraViewModel
                {
                    TotalAseguradoras = aseguradoras.Count,
                    AseguradorasActivas = aseguradoras.Count(a => a.Estado),
                    AseguradorasInactivas = aseguradoras.Count(a => !a.Estado),
                    Aseguradoras = aseguradoras,
                    // Agregar permisos al ViewModel
                    CanCreate = permissions["Create"],
                    CanUpdate = permissions["Update"],
                    CanDelete = permissions["Delete"]
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar las aseguradoras: " + ex.Message;
                return View(new AseguradoraViewModel());
            }
        }

        // GET: Aseguradora/Exportar
         public async Task<IActionResult> Exportar()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasExportPermission = await _authorizationService.HasPermissionAsync(userId, "Aseguradoras", "Export") || 
                                    await _authorizationService.HasPermissionAsync(userId, "Aseguradoras", "Read");

            if (!hasExportPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para exportar aseguradoras.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var aseguradoras = await _context.Aseguradoras
                    .Where(t => t.EmpresaId == CurrentEmpresaId)
                    .OrderBy(t => t.Codigo)
                    .ToListAsync();

                var exportConfig = _configurationService.GetExportConfiguration<Aseguradora>();
                var excelData = await _importExportService.ExportToExcelAsync(aseguradoras, exportConfig);
                var fileName = $"{exportConfig.FileName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al exportar los tipos de carnet: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Aseguradora/ImportarPlantilla
        public async Task<IActionResult> ImportarPlantilla()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasImportPermission = await _authorizationService.HasPermissionAsync(userId, "Aseguradoras", "Import") || 
                                    await _authorizationService.HasPermissionAsync(userId, "Aseguradoras", "Create");

            Console.WriteLine("Has Import Permission: " + hasImportPermission);
            if (!hasImportPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para importar aseguradoras.";
                return RedirectToAction(nameof(Index));
            }

            return View(new ImportarAseguradoraViewModel());
        }


        // GET: Aseguradora/DescargarPlantilla
        [HttpGet]
        [ActionName("DescargarPlantilla")]
        public IActionResult DescargarPlantilla()
        {
            try
            {
                var importConfig = _configurationService.GetImportConfiguration<AseguradoraItemViewModel>();
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

        // POST: Aseguradora/ImportarPlantilla
        [HttpPost]
        [ActionName("ImportarPlantilla")]
        public async Task<IActionResult> ImportarPlantilla(ImportarAseguradoraViewModel model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasImportPermission = await _authorizationService.HasPermissionAsync(userId, "Aseguradoras", "Import") || 
                                    await _authorizationService.HasPermissionAsync(userId, "Aseguradoras", "Create");

            if (!hasImportPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para importar aseguradoras.";
                return RedirectToAction(nameof(Index));
            }

            if (model.ArchivoExcel == null || model.ArchivoExcel.Length == 0)
            {
                ModelState.AddModelError("ArchivoExcel", "Debe seleccionar un archivo Excel.");
                return View(model);
            }

            try
            {
                var importConfig = _configurationService.GetImportConfiguration<AseguradoraItemViewModel>();
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

                HttpContext.Session.SetString("AseguradorasCargadas", JsonConvert.SerializeObject(importResult.Data));
                model.AseguradorasCargadas = importResult.Data;
                TempData["Success"] = $"Se procesaron {importResult.Data.Count} aseguradoras correctamente. Revise los datos y confirme la importación.";

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al procesar el archivo: " + ex.Message);
                return View(model);
            }
        }

        // POST: Aseguradora/GuardarAseguradorasImportadas
        [HttpPost]
        public async Task<IActionResult> GuardarAseguradorasImportadas()
        {
            Console.WriteLine("Guardando aseguradoras importadas...");
            var json = HttpContext.Session.GetString("AseguradorasCargadas");
            if (string.IsNullOrEmpty(json))
            {
                TempData["Error"] = "No hay datos para importar.";
                return RedirectToAction(nameof(Index));
            }
            Console.WriteLine("Datos JSON encontrados en la sesión.");
            try
            {
                var aseguradorasCargadas = JsonConvert.DeserializeObject<List<AseguradoraItemViewModel>>(json);
                if (aseguradorasCargadas == null || !aseguradorasCargadas.Any())
                {
                    TempData["Error"] = "No hay datos para importar.";
                    return RedirectToAction(nameof(Index));
                }
                Console.WriteLine($"Cantidad de aseguradoras a importar: {aseguradorasCargadas.Count}");
                var totalProcessed = await _persistenceService.SaveImportedDataAsync<Aseguradora, AseguradoraItemViewModel>(
                    aseguradorasCargadas,
                    CurrentEmpresaId,
                    // Create mapper
                    viewModel => new Aseguradora
                    {
                        Codigo = viewModel.Codigo.ToUpper(),
                        Nombre = viewModel.Nombre,
                        Descripcion = viewModel.Descripcion,
                        Estado = viewModel.Estado,
                        FechaCreacion = DateTime.UtcNow,
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
                    (viewModel, dbSet) => dbSet.FirstOrDefault(a => 
                        a.EmpresaId == CurrentEmpresaId && 
                        a.Codigo.ToUpper() == viewModel.Codigo.ToUpper())
                );

                HttpContext.Session.Remove("AseguradorasCargadas");
                TempData["Success"] = $"Importación completada: {totalProcessed} aseguradoras procesadas exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar las aseguradoras: " + ex.Message);
                TempData["Error"] = "Error al guardar las aseguradoras: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Aseguradora/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aseguradora = await _context.Aseguradoras
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

            if (aseguradora == null)
            {
                return NotFound();
            }

            var viewModel = new AseguradoraItemViewModel
            {
                Id = aseguradora.Id,
                Codigo = aseguradora.Codigo,
                Nombre = aseguradora.Nombre,
                Descripcion = aseguradora.Descripcion,
                Estado = aseguradora.Estado,
                FechaCreacion = aseguradora.FechaCreacion
            };

            return View(viewModel);
        }

        // GET: Aseguradora/Create
       public async Task<IActionResult> Create()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"); 
            var hasCreatePermission = await _authorizationService.HasPermissionAsync(userId, "Aseguradoras", "Create");
            Console.WriteLine("Has Create Permission: " + hasCreatePermission);
            if (!hasCreatePermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para exportar aseguradoras.";
                return RedirectToAction(nameof(Index));
            }

            return View( new AseguradoraCreateViewModel());  
        }

        // POST: Aseguradora/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AseguradoraCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el código ya existe
                    var existeCodigo = await _context.Aseguradoras
                        .AnyAsync(a => a.Codigo == viewModel.Codigo);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe una aseguradora con este código.");
                        return View(viewModel);
                    }

                    var aseguradora = new Aseguradora
                    {
                        Codigo = viewModel.Codigo,
                        Nombre = viewModel.Nombre,
                        Descripcion = viewModel.Descripcion,
                        Estado = viewModel.Estado,
                        FechaCreacion = DateTime.UtcNow,
                        EmpresaId = CurrentEmpresaId
                    };

                    _context.Add(aseguradora);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Aseguradora creada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al crear la aseguradora: " + ex.Message;
                }
            }

            return View(viewModel);
        }

        // GET: Aseguradora/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aseguradora = await _context.Aseguradoras.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
            if (aseguradora == null)
            {
                return NotFound();
            }

            var viewModel = new AseguradoraEditViewModel
            {
                Id = aseguradora.Id,
                Codigo = aseguradora.Codigo,
                Nombre = aseguradora.Nombre,
                Descripcion = aseguradora.Descripcion,
                Estado = aseguradora.Estado,
                FechaCreacion = aseguradora.FechaCreacion
            };

            return View(viewModel);
        }

        // POST: Aseguradora/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AseguradoraEditViewModel viewModel)
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
                    var existeCodigo = await _context.Aseguradoras
                        .AnyAsync(a => a.Codigo == viewModel.Codigo && a.Id != id);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe una aseguradora con este código.");
                        return View(viewModel);
                    }

                    var aseguradora = await _context.Aseguradoras.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                    if (aseguradora == null)
                    {
                        return NotFound();
                    }

                    aseguradora.Codigo = viewModel.Codigo;
                    aseguradora.Nombre = viewModel.Nombre;
                    aseguradora.Descripcion = viewModel.Descripcion;
                    aseguradora.Estado = viewModel.Estado;

                    _context.Update(aseguradora);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Aseguradora actualizada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AseguradoraExists(viewModel.Id))
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
                    TempData["Error"] = "Error al actualizar la aseguradora: " + ex.Message;
                }
            }

            return View(viewModel);
        }
 
        // POST: Aseguradora/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var aseguradora = await _context.Aseguradoras.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (aseguradora != null)
                {
                    _context.Aseguradoras.Remove(aseguradora);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Aseguradora eliminada exitosamente.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar la aseguradora: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Aseguradora/ToggleEstado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            try
            {
                var aseguradora = await _context.Aseguradoras.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (aseguradora != null)
                {
                    aseguradora.Estado = !aseguradora.Estado;
                    _context.Update(aseguradora);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, estado = aseguradora.Estado });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = false, message = "Aseguradora no encontrada" });
        }

        private bool AseguradoraExists(int id)
        {
            return _context.Aseguradoras.Any(e => e.Id == id);
        }
    }
}