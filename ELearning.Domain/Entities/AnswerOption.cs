using ELearning.Domain.Common;

namespace ELearning.Domain.Entities;

public class AnswerOption : BaseEntity
{
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; } = false;
    public Guid QuestionId { get; set; }

    public Question Question { get; set; } = null!;
}