using ELearning.Domain.Common;

namespace ELearning.Domain.Entities;

public class Quiz : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public int PassingScore { get; set; } = 60;
    public int? TimeLimitMinutes { get; set; }
    public bool ShuffleQuestions { get; set; } = false;
    public int MaxAttempts { get; set; } = 3;
    public Guid LessonId { get; set; }

    // Navigation Properties
    public Lesson Lesson { get; set; } = null!;
    public ICollection<Question> Questions { get; set; } = [];
    public ICollection<QuizAttempt> Attempts { get; set; } = [];
}