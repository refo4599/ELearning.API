using ELearning.Application.Common;
using ELearning.Application.DTOs.Quizzes;
using ELearning.Application.Interfaces;
using ELearning.Domain.Entities;

namespace ELearning.Infrastructure.Services;

public class QuizService : IQuizService
{
    private readonly IUnitOfWork _uow;

    public QuizService(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<QuizDto>> GetQuizByLessonAsync(Guid userId, Guid lessonId)
    {
        // التحقق من وجود الدرس والاشتراك في الكورس
        var lesson = await _uow.Lessons.GetWithSectionAsync(lessonId);
        if (lesson is null)
            return Result<QuizDto>.Failure("الدرس غير موجود.");

        var isEnrolled = await _uow.Enrollments.IsEnrolledAsync(userId, lesson.Section.CourseId);
        if (!isEnrolled && !lesson.IsFree)
            return Result<QuizDto>.Failure("يجب الاشتراك في الكورس أولاً.");

        var quiz = await _uow.Quizzes.GetByLessonIdAsync(lessonId);
        if (quiz is null)
            return Result<QuizDto>.Failure("لا يوجد كويز لهذا الدرس.");

        // جلب الكويز مع الأسئلة
        var quizWithQuestions = await _uow.Quizzes.GetWithQuestionsAsync(quiz.Id);
        if (quizWithQuestions is null)
            return Result<QuizDto>.Failure("لا يوجد كويز لهذا الدرس.");

        return Result<QuizDto>.Success(MapToDto(quizWithQuestions));
    }

    public async Task<Result<QuizResultDto>> SubmitAttemptAsync(Guid userId, Guid quizId, QuizAttemptRequest request)
    {
        var quiz = await _uow.Quizzes.GetWithQuestionsAsync(quizId);
        if (quiz is null)
            return Result<QuizResultDto>.Failure("الكويز غير موجود.");

        // التحقق من عدد المحاولات
        var attemptsCount = await _uow.Quizzes.CountAttemptsAsync(userId, quizId);
        if (attemptsCount >= quiz.MaxAttempts)
            return Result<QuizResultDto>.Failure($"لقد استنفذت الحد الأقصى للمحاولات ({quiz.MaxAttempts}).");

        // بناء lookup سريع للإجابات المرسلة
        var answersMap = request.Answers.ToDictionary(a => a.QuestionId, a => a.SelectedOptionId);

        // حساب النتيجة
        var questionResults = new List<QuestionResultDto>();
        int earnedPoints = 0;
        int totalPoints = quiz.Questions.Sum(q => q.Points);

        foreach (var question in quiz.Questions)
        {
            var correctOption = question.AnswerOptions.FirstOrDefault(o => o.IsCorrect);
            if (correctOption is null) continue;

            answersMap.TryGetValue(question.Id, out var selectedOptionId);
            var isCorrect = selectedOptionId == correctOption.Id;

            if (isCorrect)
                earnedPoints += question.Points;

            questionResults.Add(new QuestionResultDto
            {
                QuestionId = question.Id,
                QuestionText = question.QuestionText,
                SelectedOptionId = selectedOptionId,
                CorrectOptionId = correctOption.Id,
                IsCorrect = isCorrect,
                Points = question.Points,
                Explanation = question.Explanation
            });
        }

        // حساب النسبة المئوية
        int scorePercentage = totalPoints > 0
            ? (int)Math.Round((double)earnedPoints / totalPoints * 100)
            : 0;

        bool isPassed = scorePercentage >= quiz.PassingScore;

        // حفظ المحاولة
        var attempt = new QuizAttempt
        {
            UserId = userId,
            QuizId = quizId,
            Score = scorePercentage,
            IsPassed = isPassed,
            TimeTakenSeconds = request.TimeTakenSeconds,
            AttemptedAt = DateTime.UtcNow
        };

        await _uow.Quizzes.AddAttemptAsync(attempt);
        await _uow.SaveChangesAsync();

        return Result<QuizResultDto>.Success(new QuizResultDto
        {
            AttemptId = attempt.Id,
            Score = scorePercentage,
            TotalPoints = totalPoints,
            IsPassed = isPassed,
            TimeTakenSeconds = request.TimeTakenSeconds,
            AttemptedAt = attempt.AttemptedAt,
            AttemptsUsed = attemptsCount + 1,
            MaxAttempts = quiz.MaxAttempts,
            Results = questionResults
        });
    }

    public async Task<Result<IEnumerable<QuizAttemptSummaryDto>>> GetMyAttemptsAsync(Guid userId, Guid quizId)
    {
        var quiz = await _uow.Quizzes.GetByIdAsync(quizId);
        if (quiz is null)
            return Result<IEnumerable<QuizAttemptSummaryDto>>.Failure("الكويز غير موجود.");

        var attempts = await _uow.Quizzes.GetUserAttemptsAsync(userId, quizId);

        var dtos = attempts.Select(a => new QuizAttemptSummaryDto
        {
            AttemptId = a.Id,
            Score = a.Score,
            IsPassed = a.IsPassed,
            TimeTakenSeconds = a.TimeTakenSeconds,
            AttemptedAt = a.AttemptedAt
        });

        return Result<IEnumerable<QuizAttemptSummaryDto>>.Success(dtos);
    }

    private static QuizDto MapToDto(Quiz quiz) => new()
    {
        Id = quiz.Id,
        Title = quiz.Title,
        PassingScore = quiz.PassingScore,
        TimeLimitMinutes = quiz.TimeLimitMinutes,
        ShuffleQuestions = quiz.ShuffleQuestions,
        MaxAttempts = quiz.MaxAttempts,
        LessonId = quiz.LessonId,
        Questions = quiz.Questions.Select(q => new QuestionDto
        {
            Id = q.Id,
            QuestionText = q.QuestionText,
            QuestionType = q.QuestionType.ToString(),
            Points = q.Points,
            OrderIndex = q.OrderIndex,
            AnswerOptions = q.AnswerOptions.Select(o => new AnswerOptionDto
            {
                Id = o.Id,
                OptionText = o.OptionText
                // IsCorrect مش بنرجعه للـ client
            })
        })
    };
}