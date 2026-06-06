using ELearning.Domain.Common;

namespace ELearning.Domain.Entities;

public class TeacherProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Qualifications { get; set; } = string.Empty;
    public string? BankAccountNumber { get; set; }
    public string? BankName { get; set; }
    public bool IsVerified { get; set; } = false;

    // Navigation
    public User User { get; set; } = null!;
}