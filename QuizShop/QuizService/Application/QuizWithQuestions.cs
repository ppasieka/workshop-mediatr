using System.Collections;
using System.Collections.Generic;

namespace QuizService.Application;

internal class QuizWithQuestions
{
    public QuizId Id { get; init; }
    public QuizTitle Title { get; init; }
    public QuestionsCollection Questions { get; init; } = new();
}

internal class Question
{
    public QuestionId Id { get; init; }
    public QuestionText Text { get; init; }
    public AnswerId? CorrectAnswerId { get; init; }
    public AnswersCollection Answers { get; init; } = new();
}

internal class QuestionsCollection : IEnumerable<Question>
{
    private readonly List<Question> _questions = new();
    public IReadOnlyCollection<Question> Questions => _questions;
    internal void Add(Question question) => _questions.Add(question);
    internal void Add(IEnumerable<Question> questions) => _questions.AddRange(questions);
    public IEnumerator<Question> GetEnumerator() => _questions.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
internal class Answer
{    
    public AnswerId Id { get; init; }
    public AnswerText Text { get ; init; }
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