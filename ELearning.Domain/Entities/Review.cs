using ELearning.Domain.Common;

namespace ELearning.Domain.Entities;

public class Review : BaseEntity
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;
    public Course Course { get; set; } = null!;
}