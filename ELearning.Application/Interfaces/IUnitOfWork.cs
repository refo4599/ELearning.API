using ELearning.Application.Interfaces.Repositories;

namespace ELearning.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ICourseRepository Courses { get; }
    IUserRepository Users { get; }
    IEnrollmentRepository Enrollments { get; }
    ILessonProgressRepository LessonProgresses { get; }
    ICategoryRepository Categories { get; }
    ILessonRepository Lessons { get; }
    IQuizRepository Quizzes { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}