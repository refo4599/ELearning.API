namespace ELearning.Application.DTOs.Courses;

public class CreateCourseRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Level { get; set; } = "Beginner";
    public string Language { get; set; } = "Arabic";
    public Guid CategoryId { get; set; }
}