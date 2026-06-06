namespace ELearning.Application.DTOs.Quizzes;

public class QuestionAnswerRequest
{
    public Guid QuestionId { get; set; }
    public Guid SelectedOptionId { get; set; }
}

public class QuizAttemptRequest
{
    public int TimeTakenSeconds { get; set; }
    public IEnumerable<QuestionAnswerRequest> Answers { get; set; } = [];
}