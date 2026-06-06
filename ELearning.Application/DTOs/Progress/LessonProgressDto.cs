namespace ELearning.Application.DTOs.Progress;

public class LessonProgressDto
{
    public Guid LessonId { get; set; }
    public string LessonTitle { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class CourseProgressDto
{
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public double CompletionPercentage { get; set; }
    public int TotalLessons { get; set; }
    public int CompletedLessons { get; set; }
    public bool IsCourseCompleted { get; set; }
    public IEnumerable<LessonProgressDto> LessonsProgress { get; set; } = [];
}