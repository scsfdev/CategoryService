
namespace CategoryService.Application.DTOs
{
    public class CategoryDto
    {
        public Guid CategoryGuid { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
