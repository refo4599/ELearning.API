using ELearning.Domain.Common;


namespace ELearning.Domain.Entities;

public class VideoContent : BaseEntity
{
    public string VideoUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public int DurationInSeconds { get; set; }      // ← اتغير من DurationSeconds
    public string Resolution { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string CloudinaryPublicId { get; set; } = string.Empty;
    public Guid LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;
}