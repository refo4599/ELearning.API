namespace ELearning.Application.DTOs.Profile;

public class CompleteTeacherProfileRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Qualifications { get; set; } = string.Empty;
    public string? BankAccountNumber { get; set; }
    public string? BankName { get; set; }
}