
namespace CategoryService.Application.DTOs
{
    public class CategoryDto
    {
        public Guid CategoryGuid { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
