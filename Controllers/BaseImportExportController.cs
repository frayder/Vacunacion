using Microsoft.AspNetCore.Mvc;
using Highdmin.Services;
using Highdmin.Data;
using System.Security.Claims;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace Highdmin.Controllers
{
    public abstract class BaseImportExportController<TEntity, TViewModel, TItemViewModel> : BaseAuthorizationController 
        where TEntity : class 
        where TViewModel : class 
        where TItemViewModel : class, new()  // Agregamos la restricción new()
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IImportExportService _importExportService;
        protected readonly IEntityConfigurationService _configurationService;
        protected readonly IDataPersistenceService _persistenceService;

        protected abstract string ModuleName { get; }
        protected abstract string SessionKey { get; }
        protected abstract IQueryable<TEntity> GetDbSet();
        protected abstract TEntity CreateEntity(TItemViewModel viewModel, int empresaId);
        protected abstract TEntity UpdateEntity(TItemViewModel viewModel, TEntity existing);
        protected abstract TEntity? FindExisting(TItemViewModel viewModel, IQueryable<TEntity> dbSet, int empresaId);

        public BaseImportExportController(
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

        [HttpGet]
        public virtual async Task<IActionResult> Exportar()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasExportPermission = await _authorizationService.HasPermissionAsync(userId, ModuleName, "Export") || 
                                    await _authorizationService.HasPermissionAsync(userId, ModuleName, "Read");
            
            if (!hasExportPermission)
            {
                TempData["ErrorMessage"] = $"No tiene permisos para exportar {ModuleName}.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var entities = GetDbSet().Where(e => EF.Property<int>(e, "EmpresaId") == CurrentEmpresaId).ToList();
                var exportConfig = _configurationService.GetExportConfiguration<TEntity>();
                var excelData = await _importExportService.ExportToExcelAsync(entities, exportConfig);
                var fileName = $"{exportConfig.FileName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al exportar {ModuleName}: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public virtual async Task<IActionResult> ImportarPlantilla()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasImportPermission = await _authorizationService.HasPermissionAsync(userId, ModuleName, "Import") || 
                                    await _authorizationService.HasPermissionAsync(userId, ModuleName, "Create");
            
            if (!hasImportPermission)
            {
                TempData["ErrorMessage"] = $"No tiene permisos para importar {ModuleName}.";
                return RedirectToAction(nameof(Index));
            }

            return View(Activator.CreateInstance(typeof(TViewModel)));
        }

        [HttpGet]
        [ActionName("DescargarPlantilla")]
        public virtual IActionResult DescargarPlantilla()
        {
            try
            {
                var importConfig = _configurationService.GetImportConfiguration<TItemViewModel>();
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

        [HttpPost]
        [ActionName("ImportarPlantilla")]
        public virtual async Task<IActionResult> ImportarPlantilla(TViewModel model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasImportPermission = await _authorizationService.HasPermissionAsync(userId, ModuleName, "Import") || 
                                    await _authorizationService.HasPermissionAsync(userId, ModuleName, "Create");
            
            if (!hasImportPermission)
            {
                TempData["ErrorMessage"] = $"No tiene permisos para importar {ModuleName}.";
                return RedirectToAction(nameof(Index));
            }

            var fileProperty = typeof(TViewModel).GetProperty("ArchivoExcel");
            var file = fileProperty?.GetValue(model) as IFormFile;

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("ArchivoExcel", "Debe seleccionar un archivo Excel.");
                return View(model);
            }

            try
            {
                var importConfig = _configurationService.GetImportConfiguration<TItemViewModel>();
                var importResult = await _importExportService.ImportFromExcelAsync(file, importConfig);

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

                HttpContext.Session.SetString(SessionKey, JsonConvert.SerializeObject(importResult.Data));
                
                var datosProperty = typeof(TViewModel).GetProperty($"{ModuleName}Cargados") ?? 
                                 typeof(TViewModel).GetProperty("DatosCargados") ??
                                 typeof(TViewModel).GetProperties().FirstOrDefault(p => p.PropertyType == typeof(List<TItemViewModel>));
                
                datosProperty?.SetValue(model, importResult.Data);
                
                TempData["Success"] = $"Se procesaron {importResult.Data.Count} registros correctamente. Revise los datos y confirme la importación.";
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al procesar el archivo: " + ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> GuardarDatosImportados()
        {
            var json = HttpContext.Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(json))
            {
                TempData["Error"] = "No hay datos para importar.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var datosCargados = JsonConvert.DeserializeObject<List<TItemViewModel>>(json);
                if (datosCargados == null || !datosCargados.Any())
                {
                    TempData["Error"] = "No hay datos para importar.";
                    return RedirectToAction(nameof(Index));
                }

                var totalProcessed = await _persistenceService.SaveImportedDataAsync<TEntity, TItemViewModel>(
                    datosCargados,
                    CurrentEmpresaId,
                    viewModel => CreateEntity(viewModel, CurrentEmpresaId),
                    UpdateEntity,
                    (viewModel, dbSet) => FindExisting(viewModel, dbSet, CurrentEmpresaId)
                );

                HttpContext.Session.Remove(SessionKey);
                TempData["Success"] = $"Importación completada: {totalProcessed} registros procesados exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al guardar los datos: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}