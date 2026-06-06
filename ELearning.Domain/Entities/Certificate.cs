using ELearning.Domain.Common;

namespace ELearning.Domain.Entities;

public class Certificate : BaseEntity
{
    public string CertificateNumber { get; set; }
        = Guid.NewGuid().ToString("N")[..12].ToUpper();
    public string? CertificateUrl { get; set; }
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;
    public Course Course { get; set; } = null!;
}