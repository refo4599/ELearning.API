using ELearning.Domain.Common;

namespace ELearning.Domain.Entities;

public class StudentProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public int Age { get; set; }
    public string GradeLevel { get; set; } = string.Empty;
    public string School { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ParentPhone { get; set; } = string.Empty;
    public string PreferredSubjects { get; set; } = string.Empty; // JSON array

    // Navigation
    public User User { get; set; } = null!;
}