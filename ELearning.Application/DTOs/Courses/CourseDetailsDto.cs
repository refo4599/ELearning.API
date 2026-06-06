namespace ELearning.Application.DTOs.Courses;

public class CourseDetailsDto : CourseDto
{
    public List<SectionDto> Sections { get; set; } = [];
}

public class SectionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public List<LessonDto> Lessons { get; set; } = [];
}

public class LessonDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public int DurationSeconds { get; set; }
    public bool IsFree { get; set; }
}