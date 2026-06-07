using ELearning.Application.Interfaces.Repositories;
using ELearning.Domain.Entities;
using ELearning.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Persistence.Repositories;

public class QuizRepository : GenericRepository<Quiz>, IQuizRepository
{
    public QuizRepository(AppDbContext context) : base(context) { }
    public async Task AddAttemptAsync(QuizAttempt attempt)
    => await _context.QuizAttempts.AddAsync(attempt);
    public async Task<Quiz?> GetByLessonIdAsync(Guid lessonId)
        => await _context.Quizzes
            .FirstOrDefaultAsync(q => q.LessonId == lessonId);

    public async Task<Quiz?> GetWithQuestionsAsync(Guid quizId)
        => await _context.Quizzes
            .Include(q => q.Questions.OrderBy(q => q.OrderIndex))
                .ThenInclude(q => q.AnswerOptions)
            .FirstOrDefaultAsync(q => q.Id == quizId);

    public async Task<int> CountAttemptsAsync(Guid userId, Guid quizId)
        => await _context.QuizAttempts
            .CountAsync(a => a.UserId == userId && a.QuizId == quizId);

    public async Task<IEnumerable<QuizAttempt>> GetUserAttemptsAsync(Guid userId, Guid quizId)
        => await _context.QuizAttempts
            .Where(a => a.UserId == userId && a.QuizId == quizId)
            .OrderByDescending(a => a.AttemptedAt)
            .ToListAsync();
}