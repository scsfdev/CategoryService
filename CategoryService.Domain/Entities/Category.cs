﻿namespace CategoryService.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public Guid CategoryGuid { get; set; } = Guid.NewGuid();
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
