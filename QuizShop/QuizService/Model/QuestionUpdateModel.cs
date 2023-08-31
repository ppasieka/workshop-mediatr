namespace QuizService.Model;

public class QuestionUpdateModel
{
    public string? Text { get; init; }
    public int? CorrectAnswerId { get; init; }
}