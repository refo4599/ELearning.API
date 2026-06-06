using ELearning.Domain.Common;
using ELearning.Domain.Enums;
using static System.Collections.Specialized.BitVector32;

namespace ELearning.Domain.Entities;

public class Course : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public decimal Price { get; set; }
    public CourseStatus Status { get; set; } = CourseStatus.Draft;
    public CourseLevel Level { get; set; } = CourseLevel.Beginner;
    public string Language { get; set; } = "Arabic";
    public float AverageRating { get; set; } = 0;
    public int TotalStudents { get; set; } = 0;
    public Guid TeacherId { get; set; }
    public Guid CategoryId { get; set; }

    // Navigation Properties
    public User Teacher { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public ICollection<Section> Sections { get; set; } = [];
    public ICollection<Enrollment> Enrollments { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<Certificate> Certificates { get; set; } = [];
    public ICollection<LiveSession> LiveSessions { get; set; } = [];
}