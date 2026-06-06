using ELearning.Domain.Common;
using ELearning.Domain.Enums;

namespace ELearning.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Student;
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public bool IsActive { get; set; } = true;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    public bool IsProfileComplete { get; set; } = false;

    // Navigation
    public StudentProfile? StudentProfile { get; set; }
    public TeacherProfile? TeacherProfile { get; set; }
    // Navigation Properties
    public ICollection<Course> TaughtCourses { get; set; } = [];
    public ICollection<Enrollment> Enrollments { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<Certificate> Certificates { get; set; } = [];
    public ICollection<Notification> Notifications { get; set; } = [];
    public ICollection<LiveSession> HostedSessions { get; set; } = [];
}