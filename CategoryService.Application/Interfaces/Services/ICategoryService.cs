using CategoryService.Domain.Entities;

namespace CategoryService.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByGuidAsync(Guid categoryGuid);
        Task<List<Category>> GetCategoriesByIdsAsync(Guid[] ids);
        Task<Category> CreateCategoryAsync(Category category);
        Task<bool> UpdateCategoryAsync(Guid categoryGuid, Category category);
        Task<bool> DeleteCategoryAsync(Guid categoryGuid);
    }
}
