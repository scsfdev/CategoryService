using CategoryService.Domain.Entities;

namespace CategoryService.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByGuidAsync(Guid categoryGuid);
        Task CreateAsync(Category categoryCreate);
        Task UpdateAsync(Guid categoryGuid, Category categoryUpdate);
        Task DeleteAsync(Category category);
        Task<bool> SaveChangesAsync();
    }
}
