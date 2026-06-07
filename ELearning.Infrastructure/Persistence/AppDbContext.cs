using ELearning.Domain.Common;
using ELearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<VideoContent> VideoContents => Set<VideoContent>();
    public DbSet<PdfContent> PdfContents => Set<PdfContent>();
    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<AnswerOption> AnswerOptions => Set<AnswerOption>();
    public DbSet<LiveSession> LiveSessions => Set<LiveSession>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<LessonProgress> LessonProgresses => Set<LessonProgress>();
    public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<StudentProfile> StudentProfiles => Set<StudentProfile>();
    public DbSet<TeacherProfile> TeacherProfiles => Set<TeacherProfile>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Soft Delete Filters
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        modelBuilder.Entity<Course>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Lesson>().HasQueryFilter(l => !l.IsDeleted);
        modelBuilder.Entity<Section>().HasQueryFilter(s => !s.IsDeleted);
        modelBuilder.Entity<Certificate>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Enrollment>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<LessonProgress>().HasQueryFilter(lp => !lp.IsDeleted);
        modelBuilder.Entity<LiveSession>().HasQueryFilter(ls => !ls.IsDeleted);
        modelBuilder.Entity<Notification>().HasQueryFilter(n => !n.IsDeleted);
        modelBuilder.Entity<PdfContent>().HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<Quiz>().HasQueryFilter(q => !q.IsDeleted);
        modelBuilder.Entity<Question>().HasQueryFilter(q => !q.IsDeleted);      // ← جديد
        modelBuilder.Entity<AnswerOption>().HasQueryFilter(a => !a.IsDeleted);  // ← جديد
        modelBuilder.Entity<QuizAttempt>().HasQueryFilter(qa => !qa.IsDeleted);
        modelBuilder.Entity<Review>().HasQueryFilter(r => !r.IsDeleted);
        modelBuilder.Entity<VideoContent>().HasQueryFilter(v => !v.IsDeleted);
        modelBuilder.Entity<StudentProfile>().HasQueryFilter(s => !s.IsDeleted);
        modelBuilder.Entity<TeacherProfile>().HasQueryFilter(t => !t.IsDeleted);
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
        return base.SaveChangesAsync(ct);
    }
}