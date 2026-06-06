namespace ELearning.Application.DTOs.Courses;

public class UpdateCourseRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string? ThumbnailUrl { get; set; }
}