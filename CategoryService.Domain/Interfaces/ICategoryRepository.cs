using CategoryService.Domain.Entities;

namespace CategoryService.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByGuidAsync(Guid categoryGuid);
        Task<List<Category>> GetCategoriesByIdsAsync(Guid[] ids);
        Task CreateAsync(Category categoryCreate);
        Task UpdateAsync(Category categoryUpdate);
        Task DeleteAsync(Category category);
        Task<bool> SaveChangesAsync();
    }
}
