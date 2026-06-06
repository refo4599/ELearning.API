namespace ELearning.Application.DTOs.Categories;

public class UpdateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
}