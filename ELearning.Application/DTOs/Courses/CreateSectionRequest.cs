namespace ELearning.Application.DTOs.Courses;

public class CreateSectionRequest
{
    public string Title { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
}