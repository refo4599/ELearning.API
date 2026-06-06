namespace ELearning.Application.DTOs.Categories;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public Guid? ParentId { get; set; }
    public List<CategoryDto> Children { get; set; } = [];
}