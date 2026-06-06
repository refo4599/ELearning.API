using ELearning.Domain.Common;

namespace ELearning.Domain.Entities;

public class LessonProgress : BaseEntity
{
    public bool IsCompleted { get; set; } = false;
    public int WatchedSeconds { get; set; } = 0;
    public DateTime? CompletedAt { get; set; }
    public DateTime LastWatchedAt { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; set; }
    public Guid LessonId { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;
    public Lesson Lesson { get; set; } = null!;
}