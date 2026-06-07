using ELearning.Application.Interfaces;
using ELearning.Application.Interfaces.Repositories;
using ELearning.Infrastructure.Persistence.Repositories;
using ELearning.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace ELearning.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    private ICourseRepository? _courses;
    private IUserRepository? _users;
    private IEnrollmentRepository? _enrollments;
    private ICategoryRepository? _categories;
    private ILessonProgressRepository? _lessonProgresses;
    private ILessonRepository? _lessons;
    private IQuizRepository? _quizzes;

    public UnitOfWork(AppDbContext context) => _context = context;

    public ICourseRepository Courses
        => _courses ??= new CourseRepository(_context);
    public IUserRepository Users
        => _users ??= new UserRepository(_context);
    public IEnrollmentRepository Enrollments
        => _enrollments ??= new EnrollmentRepository(_context);
    public ICategoryRepository Categories
        => _categories ??= new CategoryRepository(_context);
    public ILessonProgressRepository LessonProgresses
        => _lessonProgresses ??= new LessonProgressRepository(_context);
    public ILessonRepository Lessons
        => _lessons ??= new LessonRepository(_context);
    public IQuizRepository Quizzes
        => _quizzes ??= new QuizRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync()
        => _transaction = await _context.Database.BeginTransactionAsync();

    public async Task CommitTransactionAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}