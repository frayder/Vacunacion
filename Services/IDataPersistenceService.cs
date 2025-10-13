using Highdmin.Data;
using Microsoft.EntityFrameworkCore;

namespace Highdmin.Services
{
    public interface IDataPersistenceService
    {
        Task<int> SaveImportedDataAsync<TEntity, TViewModel>(
            List<TViewModel> viewModels, 
            int empresaId,
            Func<TViewModel, TEntity> createMapper,
            Func<TViewModel, TEntity, TEntity> updateMapper,
            Func<TViewModel, IQueryable<TEntity>, TEntity?> findExisting)
            where TEntity : class
            where TViewModel : class;
    }

    public class DataPersistenceService : IDataPersistenceService
    {
        private readonly ApplicationDbContext _context;

        public DataPersistenceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveImportedDataAsync<TEntity, TViewModel>(
            List<TViewModel> viewModels, 
            int empresaId,
            Func<TViewModel, TEntity> createMapper,
            Func<TViewModel, TEntity, TEntity> updateMapper,
            Func<TViewModel, IQueryable<TEntity>, TEntity?> findExisting)
            where TEntity : class
            where TViewModel : class
        {
            var dbSet = _context.Set<TEntity>();
            int totalProcessed = 0;

            foreach (var viewModel in viewModels)
            {
                var existing = findExisting(viewModel, dbSet);
                
                if (existing != null)
                {
                    // Actualizar existente
                    updateMapper(viewModel, existing);
                    _context.Update(existing);
                }
                else
                {
                    // Crear nuevo
                    var newEntity = createMapper(viewModel);
                    dbSet.Add(newEntity);
                }
                totalProcessed++;
            }

            await _context.SaveChangesAsync();
            return totalProcessed;
        }
    }
}