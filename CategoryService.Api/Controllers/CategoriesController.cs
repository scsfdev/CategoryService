using AutoMapper;
using CategoryService.Application.DTOs;
using CategoryService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CategoryService.Api.Controllers
{
    public class CategoriesController(ICategoryService categoryService, IMapper mapper): BaseApiController
    {
        // GET: api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryMinimalDto>>> GetAllCategories()
        {
            var categories = await categoryService.GetAllCategoriesAsync();
            var categoriesDto = mapper.Map<IEnumerable<CategoryMinimalDto>>(categories);
            return Ok(categoriesDto);      // return 200 OK with the list of categories
        }

        // GET: api/categories/{categoryGuid}
        [HttpGet("{categoryGuid}")]
        public async Task<ActionResult<CategoryMinimalDto>> GetCategoryDetail([FromRoute] Guid categoryGuid)
        {
            var category = await categoryService.GetCategoryByGuidAsync(categoryGuid);
            if (category is null)
            {
                return NotFound();      // return 404 Not Found if the category does not exist
            }
            var categoryMinDto = mapper.Map<CategoryMinimalDto>(category);

            return Ok(categoryMinDto);    // return 200 OK with the category details
        }


        // Batch endpoint to get categories by multiple IDs
        [HttpGet("byIds")]
        public async Task<ActionResult<IEnumerable<CategoryBffDto>>> GetCategoriesByIds([FromQuery] Guid[] ids)
        {
            if (ids == null || ids.Length == 0)
                return BadRequest("No category IDs provided");

            var categories = await categoryService.GetCategoriesByIdsAsync(ids);

            var result = categories.Select(c => new CategoryBffDto
            {
                CategoryGuid = c.CategoryGuid,
                Title = c.Title
            }).ToList();

            return Ok(result);
        }



        // POST: api/categories
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> CreateCategory([FromBody] CategoryWriteDto categoryCreateDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);      // return 400 Bad Request if the model state is invalid
            }

            var category = mapper.Map<Domain.Entities.Category>(categoryCreateDto);
            var createdCategory = await categoryService.CreateCategoryAsync(category);
            var categoryMinDto = mapper.Map<CategoryMinimalDto>(createdCategory);

            return CreatedAtAction(nameof(GetCategoryDetail), 
                new { categoryGuid = categoryMinDto.CategoryGuid },
                categoryMinDto);       // return 201 Created with the location of the new category
        }


        // POST: api/categories/{categoryGuid}
        [Authorize(Roles = "Admin")]
        [HttpPut("{categoryGuid}")]
        public async Task<ActionResult> UpdateCategory([FromRoute] Guid categoryGuid, [FromBody] CategoryWriteDto categoryUpdateDto)
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
        [Authorize(Roles = "Admin")]
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
