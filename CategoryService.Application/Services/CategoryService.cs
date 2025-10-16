using CategoryService.Application.Interfaces.Repositories;
using CategoryService.Application.Interfaces.Services;
using CategoryService.Domain.Entities;

namespace CategoryService.Application.Services
{
    public class CategoryService(ICategoryRepository repository) : ICategoryService
    {
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await repository.GetAllAsync();
        }

        public async Task<Category?> GetCategoryByGuidAsync(Guid categoryGuid)
        {
            return await repository.GetByGuidAsync(categoryGuid);
        }

        public async Task<List<Category>> GetCategoriesByIdsAsync(Guid[] ids)
        {
            return await repository.GetCategoriesByIdsAsync(ids);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            await repository.CreateAsync(category);
            await repository.SaveChangesAsync();
            return category;
        }

        public async Task<bool> UpdateCategoryAsync(Guid categoryGuid, Category category)
        {
            var existingCategory = await repository.GetByGuidAsync(categoryGuid);
            if(existingCategory == null) return false;

            existingCategory.Title = category.Title;
            existingCategory.Description = category.Description;
            existingCategory.UpdatedAt = DateTime.UtcNow;  // Update the DT stamp.

            await repository.UpdateAsync(existingCategory);
            
            return await repository.SaveChangesAsync();
        }

        public async Task<bool> DeleteCategoryAsync(Guid categoryGuid)
        {
            var category = await repository.GetByGuidAsync(categoryGuid);
            if(category == null) return false;

            await repository.DeleteAsync(category);
            return await repository.SaveChangesAsync();
        }
       
    }
}
