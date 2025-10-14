using Microsoft.AspNetCore.Mvc;

namespace Highdmin.Services
{
    public interface IControllerImportExportService
    {
        Task<IActionResult> ExportAsync<TEntity>(
            IQueryable<TEntity> query, 
            string controllerName,
            string sessionKey = "DatosCargados") where TEntity : class;

        Task<IActionResult> ImportTemplateAsync<TViewModel>(
            string controllerName) where TViewModel : class, new();

        byte[] GenerateTemplateAsync<TViewModel>() where TViewModel : class, new();

        Task<IActionResult> ProcessImportAsync<TViewModel>(
            IFormFile file,
            string controllerName,
            string sessionKey = "DatosCargados") where TViewModel : class, new();

        Task<IActionResult> SaveImportedDataAsync<TEntity, TViewModel>(
            string sessionKey,
            string controllerName,
            int empresaId,
            Func<TViewModel, TEntity> createMapper,
            Func<TViewModel, TEntity, TEntity> updateMapper,
            Func<TViewModel, IQueryable<TEntity>, TEntity?> findExisting,
            IQueryable<TEntity> dbSet) where TEntity : class where TViewModel : class, new();
    }
}