using AutoMapper;
using CategoryService.Application.DTOs;
using CategoryService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CategoryService.UnitTests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> mockRepo;
        private readonly IMapper mapper;
        private readonly Application.Services.CategoryService service;

        public CategoryServiceTests()
        {
            mockRepo = new Mock<ICategoryRepository>();
            // ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Domain.Entities.Category, CategoryDto>();
                cfg.CreateMap<CategoryCreateDto, Domain.Entities.Category>();
            }, loggerFactory);

            mapper = config.CreateMapper();
            service = new Application.Services.CategoryService(mockRepo.Object, mapper);
        }

        [Fact]
        public void TestSetupWorks()
        {
            Assert.NotNull(mockRepo);
            Assert.NotNull(mapper);
            Assert.NotNull(service);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ReturnsMappedCategories()
        {
            // Arrange
            var categories = new List<Domain.Entities.Category>
            {
                new() { CategoryGuid = Guid.NewGuid(), Title = "Category1", Description = "Description1" },
                new() { CategoryGuid = Guid.NewGuid(), Title = "Category2", Description = "Description2" }
            };

            // Set up the mock repository to return the categories.
            mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(categories);

            // Call the service method.
            var result = await service.GetAllCategoriesAsync();

            // Assert (Verify)
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("Category1", result.First().Title);
            mockRepo.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetCategoryByGuidAsync_CategoryExists_ReturnsMappedCategory()
        {
            // Arrange
            var categoryGuid = Guid.NewGuid();
            var category = new Domain.Entities.Category { CategoryGuid = categoryGuid, Title = "Category1", Description = "Description1" };
            mockRepo.Setup(repo => repo.GetByGuidAsync(categoryGuid)).ReturnsAsync(category);
            // Act
            var result = await service.GetCategoryByGuidAsync(categoryGuid);
            // Assert
            Assert.NotNull(result);
            Assert.Equal("Category1", result.Title);
            mockRepo.Verify(repo => repo.GetByGuidAsync(categoryGuid), Times.Once);
        }


        [Fact]
        public async Task UpdateCategoryAsync_ReturnsTrue_WhenCategoryExists()
        {
            var existingCategory = new Domain.Entities.Category
            {
                CategoryGuid = Guid.NewGuid(),
                Title = "Old Title",
                Description = "Old Description"
            };

            var updateDto = new CategoryCreateDto
            {
                Title = "New Title",
                Description = "New Description"
            };

            mockRepo.Setup(r => r.GetByGuidAsync(existingCategory.CategoryGuid)).ReturnsAsync(existingCategory);
            mockRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
            mockRepo.Setup(r => r.UpdateAsync(existingCategory.CategoryGuid, It.IsAny<Domain.Entities.Category>())).Returns(Task.CompletedTask);

            var result = await service.UpdateCategoryAsync(existingCategory.CategoryGuid, updateDto);

            // Check state first.
            Assert.True(result);
            Assert.Equal("New Title", existingCategory.Title);
            Assert.Equal("New Description", existingCategory.Description);

            // Then check interactions.
            mockRepo.Verify(r => r.GetByGuidAsync(existingCategory.CategoryGuid), Times.Once);
            mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateCategoryAsync_ReturnFalse_WhenCategoryNotFound()
        {
            var updateDto = new CategoryCreateDto
            {
                Title = "New Title",
                Description = "New Description"
            };

            mockRepo.Setup(r=> r.GetByGuidAsync(It.IsAny<Guid>())).ReturnsAsync((Domain.Entities.Category?)null);

            var result = await service.UpdateCategoryAsync(Guid.NewGuid(), updateDto);

            Assert.False(result);

            mockRepo.Verify(r => r.GetByGuidAsync(It.IsAny<Guid>()), Times.Once);
            mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}
