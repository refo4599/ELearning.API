namespace ELearning.Application.DTOs.Categories;

public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public Guid? ParentId { get; set; }
}