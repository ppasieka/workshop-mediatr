#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace QuizService.Application;

internal class Quiz
{
    public QuizId Id { get; }
    public QuizTitle Title { get; }

    private Quiz() {}
    
    public Quiz(QuizId id, QuizTitle title) : this()
    {
        Id = id;
        Title = title;
    }
}