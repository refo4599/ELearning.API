namespace ELearning.Application.DTOs.Profile;

public class CompleteStudentProfileRequest
{
    public string FullName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string GradeLevel { get; set; } = string.Empty;
    public string School { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ParentPhone { get; set; } = string.Empty;
    public List<string> PreferredSubjects { get; set; } = [];
}