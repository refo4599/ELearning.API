using ELearning.Domain.Common;

namespace ELearning.Domain.Entities;

public class QuizAttempt : BaseEntity
{
    public int Score { get; set; }
    public bool IsPassed { get; set; }
    public int TimeTakenSeconds { get; set; }
    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; set; }
    public Guid QuizId { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;
    public Quiz Quiz { get; set; } = null!;
}