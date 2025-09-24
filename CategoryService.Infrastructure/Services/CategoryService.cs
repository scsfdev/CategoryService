using CategoryService.Application.Interfaces;
using CategoryService.Domain.Entities;
using CategoryService.Domain.Interfaces;


namespace CategoryService.Infrastructure.Services
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

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            await repository.CreateAsync(category);
            await repository.SaveChangesAsync();
            return category;
        }

        public async Task<bool> UpdateCategoryAsync(Guid categoryGuid, Category category)
        {
            var cat = await repository.GetByGuidAsync(categoryGuid);
            if(cat == null) return false;

            await repository.UpdateAsync(categoryGuid, category);
            
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
