using ELearning.Domain.Common;
using ELearning.Domain.Enums;

namespace ELearning.Domain.Entities;

public class LiveSession : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; }
    public string? MeetingUrl { get; set; }
    public LiveSessionStatus Status { get; set; } = LiveSessionStatus.Scheduled;
    public int MaxAttendees { get; set; } = 100;
    public Guid CourseId { get; set; }
    public Guid TeacherId { get; set; }

    // Navigation Properties
    public Course Course { get; set; } = null!;
    public User Teacher { get; set; } = null!;
}