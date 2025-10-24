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
    public class TipoCarnetController : BaseAuthorizationController
    {
        private readonly ApplicationDbContext _context;
        private readonly IImportExportService _importExportService;
        private readonly IEntityConfigurationService _configurationService;
        private readonly IDataPersistenceService _persistenceService;

        public TipoCarnetController(
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

        // GET: TipoCarnet
        public async Task<IActionResult> Index()
        {
            // Validar permisos y obtener todos los permisos del módulo
            var (redirect, permissions) = await ValidateAndGetPermissionsAsync("TipoCarnet", "Read");
            if (redirect != null) return redirect;

            try
            {
                var tiposCarnet = await _context.TiposCarnet
                    .Where(t => t.EmpresaId == CurrentEmpresaId)
                    .OrderBy(t => t.Codigo)
                    .Select(t => new TipoCarnetItemViewModel
                    {
                        Id = t.Id,
                        Codigo = t.Codigo,
                        Nombre = t.Nombre,
                        Descripcion = t.Descripcion,
                        Estado = t.Estado,
                        FechaCreacion = t.FechaCreacion
                    })
                    .ToListAsync();

                var viewModel = new TipoCarnetViewModel
                {
                    TotalTipos = tiposCarnet.Count,
                    TiposActivos = tiposCarnet.Count(t => t.Estado),
                    TiposInactivos = tiposCarnet.Count(t => !t.Estado),
                    TiposCarnet = tiposCarnet,
                    // Agregar permisos al ViewModel
                    CanCreate = permissions["Create"],
                    CanUpdate = permissions["Update"],
                    CanDelete = permissions["Delete"]
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los tipos de carnet: " + ex.Message;
                return View(new TipoCarnetViewModel());
            }
        }

        // GET: TipoCarnet/Exportar
        public async Task<IActionResult> Exportar()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasExportPermission = await _authorizationService.HasPermissionAsync(userId, "TipoCarnet", "Export") || 
                                    await _authorizationService.HasPermissionAsync(userId, "TipoCarnet", "Read");
            
            if (!hasExportPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para exportar tipos de carnet.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var tiposCarnet = await _context.TiposCarnet
                    .Where(t => t.EmpresaId == CurrentEmpresaId)
                    .OrderBy(t => t.Codigo)
                    .ToListAsync();

                var exportConfig = _configurationService.GetExportConfiguration<TipoCarnet>();
                var excelData = await _importExportService.ExportToExcelAsync(tiposCarnet, exportConfig);
                var fileName = $"{exportConfig.FileName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al exportar los tipos de carnet: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: TipoCarnet/ImportarPlantilla
        public async Task<IActionResult> ImportarPlantilla()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasImportPermission = await _authorizationService.HasPermissionAsync(userId, "TipoCarnet", "Import") || 
                                    await _authorizationService.HasPermissionAsync(userId, "TipoCarnet", "Create");
            
            if (!hasImportPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para importar tipos de carnet.";
                return RedirectToAction(nameof(Index));
            }

            return View(new ImportarTipoCarnetViewModel());
        }

        // GET: TipoCarnet/DescargarPlantilla
        [HttpGet]
        [ActionName("DescargarPlantilla")]
        public IActionResult DescargarPlantilla()
        {
            try
            {
                var importConfig = _configurationService.GetImportConfiguration<TipoCarnetItemViewModel>();
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

        // POST: TipoCarnet/ImportarPlantilla
        [HttpPost]
        [ActionName("ImportarPlantilla")]
        public async Task<IActionResult> ImportarPlantilla(ImportarTipoCarnetViewModel model)
        {
            Console.WriteLine("Iniciando importación de plantilla.. tipocarnet.");
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasImportPermission = await _authorizationService.HasPermissionAsync(userId, "TipoCarnet", "Import") || 
                                    await _authorizationService.HasPermissionAsync(userId, "TipoCarnet", "Create");
            
            if (!hasImportPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para importar tipos de carnet.";
                return RedirectToAction(nameof(Index));
            }

            if (model.ArchivoExcel == null || model.ArchivoExcel.Length == 0)
            {
                ModelState.AddModelError("ArchivoExcel", "Debe seleccionar un archivo Excel.");
                return View(model);
            }

            try
            {
                var importConfig = _configurationService.GetImportConfiguration<TipoCarnetItemViewModel>();
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
                HttpContext.Session.SetString("TiposCargados", JsonConvert.SerializeObject(importResult.Data));
                model.TiposCarnetCargados = importResult.Data;
                TempData["Success"] = $"Se procesaron {importResult.Data.Count} tipos de carnet correctamente. Revise los datos y confirme la importación.";

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al procesar el archivo: " + ex.Message);
                return View(model);
            }
        }

        // POST: TipoCarnet/GuardarTiposImportados
        [HttpPost]
        public async Task<IActionResult> GuardarTiposImportados()
        {
            Console.WriteLine("Guardando tipos importados...");
            var json = HttpContext.Session.GetString("TiposCargados");
            if (string.IsNullOrEmpty(json))
            {
                TempData["Error"] = "No hay datos para importar.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var tiposCargados = JsonConvert.DeserializeObject<List<TipoCarnetItemViewModel>>(json);
                if (tiposCargados == null || !tiposCargados.Any())
                {
                    TempData["Error"] = "No hay datos para importar.";
                    return RedirectToAction(nameof(Index));
                }

                var totalProcessed = await _persistenceService.SaveImportedDataAsync<TipoCarnet, TipoCarnetItemViewModel>(
                    tiposCargados,
                    CurrentEmpresaId,
                    // Create mapper
                    viewModel => new TipoCarnet
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
                    (viewModel, dbSet) => dbSet.FirstOrDefault(t => 
                        t.EmpresaId == CurrentEmpresaId && 
                        t.Codigo.ToUpper() == viewModel.Codigo.ToUpper())
                );

                HttpContext.Session.Remove("TiposCargados");
                TempData["Success"] = $"Importación completada: {totalProcessed} tipos de carnet procesados exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al guardar los tipos de carnet: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: TipoCarnet/Crear
        public async Task<IActionResult> Crear()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasCreatePermission = await _authorizationService.HasPermissionAsync(userId, "TipoCarnet", "Create");
            
            if (!hasCreatePermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para crear tipos de carnet.";
                return RedirectToAction(nameof(Index));
            }

            return View(new TipoCarnetCreateViewModel());
        }

        // POST: TipoCarnet/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(TipoCarnetCreateViewModel model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasCreatePermission = await _authorizationService.HasPermissionAsync(userId, "TipoCarnet", "Create");
            
            if (!hasCreatePermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para crear tipos de carnet.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el código ya existe
                    var existeCodigo = await _context.TiposCarnet
                        .AnyAsync(t => t.Codigo.ToUpper() == model.Codigo.ToUpper());

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe un tipo de carnet con este código");
                        return View(model);
                    }

                    var tipoCarnet = new TipoCarnet
                    {
                        Codigo = model.Codigo.ToUpper(),
                        Nombre = model.Nombre,
                        Descripcion = model.Descripcion,
                        Estado = model.Estado,
                        FechaCreacion = DateTime.UtcNow,
                        EmpresaId = CurrentEmpresaId
                    };

                    _context.TiposCarnet.Add(tipoCarnet);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Tipo de carnet creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear el tipo de carnet: " + ex.InnerException?.Message);
                }
            }

            return View(model);
        }

        // GET: TipoCarnet/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasUpdatePermission = await _authorizationService.HasPermissionAsync(userId, "TipoCarnet", "Update");
            
            if (!hasUpdatePermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para editar tipos de carnet.";
                return RedirectToAction(nameof(Index));
            }

            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var tipoCarnet = await _context.TiposCarnet.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (tipoCarnet == null)
                {
                    return NotFound();
                }

                var model = new TipoCarnetEditViewModel
                {
                    Id = tipoCarnet.Id,
                    Codigo = tipoCarnet.Codigo,
                    Nombre = tipoCarnet.Nombre,
                    Descripcion = tipoCarnet.Descripcion,
                    Estado = tipoCarnet.Estado,
                    FechaCreacion = tipoCarnet.FechaCreacion
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el tipo de carnet: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: TipoCarnet/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, TipoCarnetEditViewModel model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasUpdatePermission = await _authorizationService.HasPermissionAsync(userId, "TipoCarnet", "Update");
            
            if (!hasUpdatePermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para editar tipos de carnet.";
                return RedirectToAction(nameof(Index));
            }

            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el código ya existe en otro registro
                    var existeCodigo = await _context.TiposCarnet
                        .AnyAsync(t => t.Codigo.ToUpper() == model.Codigo.ToUpper() && t.Id != id);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe un tipo de carnet con este código");
                        return View(model);
                    }

                    var tipoCarnet = await _context.TiposCarnet.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                    if (tipoCarnet == null)
                    {
                        return NotFound();
                    }

                    tipoCarnet.Codigo = model.Codigo.ToUpper();
                    tipoCarnet.Nombre = model.Nombre;
                    tipoCarnet.Descripcion = model.Descripcion;
                    tipoCarnet.Estado = model.Estado;

                    _context.Update(tipoCarnet);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Tipo de carnet actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoCarnetExists(model.Id))
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
                    ModelState.AddModelError("", "Error al actualizar el tipo de carnet: " + ex.Message);
                }
            }

            return View(model);
        }

        // POST: TipoCarnet/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasDeletePermission = await _authorizationService.HasPermissionAsync(userId, "TipoCarnet", "Delete");
            
            if (!hasDeletePermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para eliminar tipos de carnet.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var tipoCarnet = await _context.TiposCarnet.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (tipoCarnet != null)
                {
                    _context.TiposCarnet.Remove(tipoCarnet);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Tipo de carnet eliminado exitosamente";
                }
                else
                {
                    TempData["Error"] = "No se encontró el tipo de carnet";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el tipo de carnet: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TipoCarnetExists(int id)
        {
            return _context.TiposCarnet.Any(e => e.Id == id);
        }
    }
}