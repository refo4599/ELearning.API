using ELearning.Application.Common;
using ELearning.Application.DTOs.Quizzes;

namespace ELearning.Application.Interfaces;

public interface IQuizService
{
    Task<Result<QuizDto>> GetQuizByLessonAsync(Guid userId, Guid lessonId);
    Task<Result<QuizResultDto>> SubmitAttemptAsync(Guid userId, Guid quizId, QuizAttemptRequest request);
    Task<Result<IEnumerable<QuizAttemptSummaryDto>>> GetMyAttemptsAsync(Guid userId, Guid quizId);
}