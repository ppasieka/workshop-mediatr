using System.Collections.Generic;

namespace QuizService.Application;

internal class QuizWithQuestions
{
    public QuizId Id { get; init; }
    public QuizTitle Title { get; init; }
    public IReadOnlyList<Question> Questions { get; init; }
}

internal class Question
{
    public QuestionId Id { get; init; }
    public QuestionText Text { get; init; }
    public AnswerId? CorrectAnswerId { get; init; }
    public IReadOnlyList<Answer> Answers { get; init; }
}

internal class Answer
{    
    public AnswerId Id { get; init; }
    public AnswerText Text { get ; init; }
}