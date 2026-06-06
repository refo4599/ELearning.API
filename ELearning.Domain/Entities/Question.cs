using ELearning.Domain.Common;
using ELearning.Domain.Enums;

namespace ELearning.Domain.Entities;

public class Question : BaseEntity
{
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; }
    public int Points { get; set; } = 1;
    public int OrderIndex { get; set; }
    public string? Explanation { get; set; }
    public Guid QuizId { get; set; }

    // Navigation Properties
    public Quiz Quiz { get; set; } = null!;
    public ICollection<AnswerOption> AnswerOptions { get; set; } = [];
}