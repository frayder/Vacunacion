using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Highdmin.Services
{
    public class ControllerImportExportService : IControllerImportExportService
    {
        private readonly IImportExportService _importExportService;
        private readonly IEntityConfigurationService _configurationService;
        private readonly IDataPersistenceService _persistenceService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ControllerImportExportService(
            IImportExportService importExportService,
            IEntityConfigurationService configurationService,
            IDataPersistenceService persistenceService,
            IHttpContextAccessor httpContextAccessor)
        {
            _importExportService = importExportService;
            _configurationService = configurationService;
            _persistenceService = persistenceService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> ExportAsync<TEntity>(
            IQueryable<TEntity> query, 
            string controllerName,
            string sessionKey = "DatosCargados") where TEntity : class
        {
            try
            {
                var entities = await query.ToListAsync();
                var exportConfig = _configurationService.GetExportConfiguration<TEntity>();
                var excelData = await _importExportService.ExportToExcelAsync(entities, exportConfig);
                var fileName = $"{exportConfig.FileName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return new FileContentResult(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = fileName
                };
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Error al exportar: " + ex.Message });
            }
        }

        public async Task<IActionResult> ImportTemplateAsync<TViewModel>(
            string controllerName) where TViewModel : class, new()
        {
            // Esta implementación requiere un controlador específico, 
            // por lo que se mantiene en los controladores individuales
            await Task.CompletedTask;
            throw new NotImplementedException("Este método debe implementarse en el controlador específico");
        }

        public byte[] GenerateTemplateAsync<TViewModel>() where TViewModel : class, new()
        {
            try
            {
                var importConfig = _configurationService.GetImportConfiguration<TViewModel>();
                return _importExportService.GenerateImportTemplate(importConfig);
            }
            catch (Exception)
            {
                return Array.Empty<byte>();
            }
        }

        public async Task<IActionResult> ProcessImportAsync<TViewModel>(
            IFormFile file,
            string controllerName,
            string sessionKey = "DatosCargados") where TViewModel : class, new()
        {
            if (file == null || file.Length == 0)
            {
                return new JsonResult(new { success = false, message = "Debe seleccionar un archivo Excel." });
            }

            try
            {
                var importConfig = _configurationService.GetImportConfiguration<TViewModel>();
                var importResult = await _importExportService.ImportFromExcelAsync(file, importConfig);

                if (importResult.HasErrors)
                {
                    return new JsonResult(new { success = false, errors = importResult.Errors });
                }

                if (!importResult.Data.Any())
                {
                    return new JsonResult(new { success = false, message = "No se encontraron datos válidos para importar." });
                }

                // Guardar en sesión
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    httpContext.Session.SetString(sessionKey, JsonConvert.SerializeObject(importResult.Data));
                }

                return new JsonResult(new { 
                    success = true, 
                    message = $"Se procesaron {importResult.Data.Count} registros correctamente.",
                    data = importResult.Data 
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Error al procesar el archivo: " + ex.Message });
            }
        }

        public async Task<IActionResult> SaveImportedDataAsync<TEntity, TViewModel>(
            string sessionKey,
            string controllerName,
            int empresaId,
            Func<TViewModel, TEntity> createMapper,
            Func<TViewModel, TEntity, TEntity> updateMapper,
            Func<TViewModel, IQueryable<TEntity>, TEntity?> findExisting,
            IQueryable<TEntity> dbSet) where TEntity : class where TViewModel : class, new()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return new JsonResult(new { success = false, message = "No se pudo acceder al contexto HTTP." });
            }

            var json = httpContext.Session.GetString(sessionKey);
            if (string.IsNullOrEmpty(json))
            {
                return new JsonResult(new { success = false, message = "No hay datos para importar." });
            }

            try
            {
                var datosCargados = JsonConvert.DeserializeObject<List<TViewModel>>(json);
                if (datosCargados == null || !datosCargados.Any())
                {
                    return new JsonResult(new { success = false, message = "No hay datos para importar." });
                }

                var totalProcessed = await _persistenceService.SaveImportedDataAsync<TEntity, TViewModel>(
                    datosCargados,
                    empresaId,
                    createMapper,
                    updateMapper,
                    (viewModel, dbSetQuery) => findExisting(viewModel, dbSetQuery)
                );

                httpContext.Session.Remove(sessionKey);

                return new JsonResult(new { 
                    success = true, 
                    message = $"Importación completada: {totalProcessed} registros procesados exitosamente." 
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "Error al guardar los datos: " + ex.Message });
            }
        }
    }
}