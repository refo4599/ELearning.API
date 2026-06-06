namespace ELearning.Application.DTOs.Quizzes;

public class AnswerOptionDto
{
    public Guid Id { get; set; }
    public string OptionText { get; set; } = string.Empty;
}

public class QuestionDto
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public int Points { get; set; }
    public int OrderIndex { get; set; }
    public IEnumerable<AnswerOptionDto> AnswerOptions { get; set; } = [];
}

public class QuizDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int PassingScore { get; set; }
    public int? TimeLimitMinutes { get; set; }
    public bool ShuffleQuestions { get; set; }
    public int MaxAttempts { get; set; }
    public Guid LessonId { get; set; }
    public IEnumerable<QuestionDto> Questions { get; set; } = [];
}