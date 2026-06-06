using ELearning.Domain.Entities;

namespace ELearning.Application.Interfaces.Repositories;

public interface IQuizRepository : IGenericRepository<Quiz>
{
    Task<Quiz?> GetByLessonIdAsync(Guid lessonId);
    Task<Quiz?> GetWithQuestionsAsync(Guid quizId);
    Task<int> CountAttemptsAsync(Guid userId, Guid quizId);
    Task<IEnumerable<QuizAttempt>> GetUserAttemptsAsync(Guid userId, Guid quizId);
    Task AddAttemptAsync(QuizAttempt attempt); // ← أضف ده
}