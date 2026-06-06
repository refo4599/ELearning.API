namespace ELearning.Application.DTOs.Courses;

public class CourseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public float AverageRating { get; set; }
    public int TotalStudents { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public Guid TeacherId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
}