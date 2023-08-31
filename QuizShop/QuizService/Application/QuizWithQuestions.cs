using System.Collections;
using System.Collections.Generic;

namespace QuizService.Application;

internal class QuizWithQuestions
{
    public required QuizId Id { get; init; }
    public required QuizTitle Title { get; init; }
    public QuestionsCollection Questions { get; init; } = new();
}

internal class Question
{
    public required QuestionId Id { get; init; }
    public required QuestionText Text { get; init; }
    public AnswerId? CorrectAnswerId { get; init; }
    public AnswersCollection Answers { get; init; } = new();
}

internal class QuestionsCollection : IEnumerable<Question>
{
    private readonly List<Question> _questions = new();
    private readonly Dictionary<QuestionId, Question> _questionLookup = new();
    public IReadOnlyCollection<Question> Questions => _questions;
    internal void Add(Question question)
    {
        _questions.Add(question);
        _questionLookup.Add(question.Id, question);
    }

    internal void Add(IEnumerable<Question> questions)
    {
        foreach (var question in questions)
        {
            Add(question);
        }
    }

    public IEnumerator<Question> GetEnumerator() => _questions.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    internal bool TryAddAnswer(Answer answer)
    {
        if (!_questionLookup.TryGetValue(answer.QuestionId, out var question))
            return false;
        question.Answers.Add(answer);
        return true;
    }
}

internal class Answer
{    
    public required AnswerId Id { get; init; }
    public required AnswerText Text { get ; init; }
    internal required QuestionId QuestionId { get; init; }
}

internal class AnswersCollection : IEnumerable<Answer>
{
    private readonly List<Answer> _answers = new();
    public IReadOnlyCollection<Answer> Answers => _answers;
    internal void Add(Answer answer) => _answers.Add(answer);
    internal void Add(IEnumerable<Answer> answers) => _answers.AddRange(answers);
    public IEnumerator<Answer> GetEnumerator() => _answers.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}