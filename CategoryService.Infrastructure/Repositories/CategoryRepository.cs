using CategoryService.Domain.Interfaces;
using CategoryService.Domain.Entities;
using CategoryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CategoryService.Infrastructure.Repositories
{
    public class CategoryRepository(CategoryDbContext categoryDb) : ICategoryRepository
    {
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await categoryDb.Categories.ToListAsync();
        }

        public async Task<Category?> GetByGuidAsync(Guid categoryGuid)
        {
            return await categoryDb.Categories.FirstOrDefaultAsync(c => c.CategoryGuid == categoryGuid);
        }

        public async Task CreateAsync(Category categoryCreateDto)
        {
            await categoryDb.Categories.AddAsync(categoryCreateDto);
        }

        // Even though Delete and Update are synchronous operations, we keep the async signature
        // to maintain consistency with the interface and allow for future async operations if needed.
        public Task DeleteAsync(Category category)
        {
            categoryDb.Categories.Remove(category);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Guid categoryGuid, Category categoryUpdateDto)
        {
            categoryDb.Categories.Update(categoryUpdateDto);
            return Task.CompletedTask;
        }

        // Unit of work principle: SaveChangesAsync to commit all changes.
        // We can add/update/dalete multiple entities and then call SaveChangesAsync once.
        public async Task<bool> SaveChangesAsync()
        {
            return await categoryDb.SaveChangesAsync() > 0;
        }
    }
}
