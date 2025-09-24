using AutoMapper;
using CategoryService.Application.DTOs;
using CategoryService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CategoryService.Api.Controllers
{
    public class CategoriesController(ICategoryService categoryService, IMapper mapper): BaseApiController
    {
        // GET: api/categories
        [HttpGet]
        public async Task<ActionResult<List<CategoryDto>>> GetAllCategories()
        {
            var categories = await categoryService.GetAllCategoriesAsync();
            var categoriesDto = mapper.Map<List<CategoryDto>>(categories);
            return categoriesDto;      // return 200 OK with the list of categories
        }

        // GET: api/categories/{categoryGuid}
        [HttpGet("{categoryGuid}")]
        public async Task<ActionResult<CategoryDto>> GetCategoryDetail([FromRoute] Guid categoryGuid)
        {
            var category = await categoryService.GetCategoryByGuidAsync(categoryGuid);
            if (category is null)
            {
                return NotFound();      // return 404 Not Found if the category does not exist
            }
            var categoryDto = mapper.Map<CategoryDto>(category);

            return Ok(categoryDto);    // return 200 OK with the category details
        }

        // POST: api/categories
        [HttpPost]
        public async Task<ActionResult> CreateCategory([FromBody] CategoryCreateDto categoryCreateDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);      // return 400 Bad Request if the model state is invalid
            }

            var category = mapper.Map<Domain.Entities.Category>(categoryCreateDto);
            var createdCategory = await categoryService.CreateCategoryAsync(category);
            var categoryDto = mapper.Map<CategoryDto>(createdCategory);

            return CreatedAtAction(nameof(GetCategoryDetail), 
                new { categoryGuid = categoryDto.CategoryGuid },
                categoryDto);       // return 201 Created with the location of the new category
        }

        // POST: api/categories/{categoryGuid}
        [HttpPut("{categoryGuid}")]
        public async Task<ActionResult<CategoryDto>> UpdateCategory([FromRoute] Guid categoryGuid, [FromBody] CategoryCreateDto categoryUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);      // return 400 Bad Request if the model state is invalid
            }
            var category = mapper.Map<Domain.Entities.Category>(categoryUpdateDto);
            var updatedCategory = await categoryService.UpdateCategoryAsync(categoryGuid, category);
            if (!updatedCategory)
            {
                return NotFound();      // return 404 Not Found if the category does not exist
            }

            return NoContent();   // return 204 No Content to indicate the update was successful
        }

        // DELETE: api/categories/{categoryGuid}
        [HttpDelete("{categoryGuid}")]
        public async Task<ActionResult> DeleteCategory([FromRoute] Guid categoryGuid)
        {
            var isDeleted = await categoryService.DeleteCategoryAsync(categoryGuid);
            if (!isDeleted)
            {
                return NotFound();  // return 404 Not Found if the category does not exist
            }

            return NoContent();     // return 204 No Content to indicate the deletion was successful
        }
    }
}
