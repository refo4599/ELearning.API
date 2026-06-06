using ELearning.Domain.Enums;

namespace ELearning.Application.DTOs.Courses;

public class CreateLessonRequest
{
    public string Title { get; set; } = string.Empty;
    public ContentType ContentType { get; set; }
    public int OrderIndex { get; set; }
    public int DurationSeconds { get; set; }
    public bool IsFree { get; set; } = false;
}