namespace ELearning.Application.DTOs.Profile;

public class StudentProfileDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public int Age { get; set; }
    public string GradeLevel { get; set; } = string.Empty;
    public string School { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ParentPhone { get; set; } = string.Empty;
    public List<string> PreferredSubjects { get; set; } = [];
    public bool IsProfileComplete { get; set; }
}

public class TeacherProfileDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Qualifications { get; set; } = string.Empty;
    public string? BankAccountNumber { get; set; }
    public string? BankName { get; set; }
    public bool IsVerified { get; set; }
    public bool IsProfileComplete { get; set; }
}