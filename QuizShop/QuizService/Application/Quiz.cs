#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace QuizService.Application;

internal class Quiz
{
    public required QuizId Id { get; init; }
    public required QuizTitle Title { get; init; }
}