namespace ELearning.Application.DTOs.Enrollments;

public class EnrollmentDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string? CourseThumbnailUrl { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
    public double CompletionPercentage { get; set; }
    public bool IsCompleted { get; set; }
}