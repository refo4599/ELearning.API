using ELearning.Domain.Common;

namespace ELearning.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public Guid? ParentId { get; set; }

    // Navigation Properties
    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; } = [];
    public ICollection<Course> Courses { get; set; } = [];
}