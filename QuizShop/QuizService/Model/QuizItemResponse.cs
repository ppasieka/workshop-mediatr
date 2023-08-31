using System.Collections.Generic;

namespace QuizService.Model;

public class QuizItemResponse
{
    public required long Id { get; init; }
    public required string Title { get; init; }
    public required IDictionary<string, string> Links { get; init; }
}

public class QuizResponse
{
    public required long Id { get; init; }
    public required string Title { get; init; }
    public required IEnumerable<QuestionItemResponse> Questions { get; init; }
    public required Dictionary<string, string> Links { get; init; }
}
    
public class AnswerItemResponse
{
    public required long Id { get; init; }
    public required string Text { get; init; }
}

public class QuestionItemResponse
{
    public required long Id { get; init; }
    public required string Text { get; init; }
    public required IEnumerable<AnswerItemResponse> Answers { get; init; }
    public required long? CorrectAnswerId { get; init; }
}
