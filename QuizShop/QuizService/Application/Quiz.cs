#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace QuizService.Application;

internal class Quiz
{
    public QuizId Id { get; }
    public string Title { get; }
    public string Description { get; }

    private Quiz() {}
    
    public Quiz(QuizId id, string title, string description) : this()
    {
        Id = id;
        Title = title;
        Description = description;
    }
}