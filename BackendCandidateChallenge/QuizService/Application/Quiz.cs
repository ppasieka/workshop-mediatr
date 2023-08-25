namespace QuizService.Application;

internal class Quiz
{
    public QuizId Id { get; }
    public string Title { get; }
    public string Description { get; }

    private Quiz() {}
    
    public Quiz(QuizId id, string title, string description)
    {
        Id = id;
        Title = title;
        Description = description;
    }
}