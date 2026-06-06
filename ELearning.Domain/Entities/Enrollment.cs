using ELearning.Domain.Common;
using ELearning.Domain.Enums;

namespace ELearning.Domain.Entities;

public class Enrollment : BaseEntity
{
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public PaymentStatus PaymentStatus { get; set; }
    public decimal PaidAmount { get; set; }
    public float CompletionPercentage { get; set; } = 0;
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;
    public Course Course { get; set; } = null!;
}