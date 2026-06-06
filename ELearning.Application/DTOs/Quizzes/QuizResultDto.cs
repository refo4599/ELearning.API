namespace ELearning.Application.DTOs.Quizzes;

public class QuestionResultDto
{
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public Guid SelectedOptionId { get; set; }
    public Guid CorrectOptionId { get; set; }
    public bool IsCorrect { get; set; }
    public int Points { get; set; }
    public string? Explanation { get; set; }
}

public class QuizResultDto
{
    public Guid AttemptId { get; set; }
    public int Score { get; set; }
    public int TotalPoints { get; set; }
    public bool IsPassed { get; set; }
    public int TimeTakenSeconds { get; set; }
    public DateTime AttemptedAt { get; set; }
    public int AttemptsUsed { get; set; }
    public int MaxAttempts { get; set; }
    public IEnumerable<QuestionResultDto> Results { get; set; } = [];
}

public class QuizAttemptSummaryDto
{
    public Guid AttemptId { get; set; }
    public int Score { get; set; }
    public bool IsPassed { get; set; }
    public int TimeTakenSeconds { get; set; }
    public DateTime AttemptedAt { get; set; }
}