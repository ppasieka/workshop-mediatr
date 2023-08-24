
using System;

namespace QuizService.Application;

internal class QuizId : IEquatable<QuizId>
{
    public int Value { get; }
    
    private QuizId(int value)
    {
        Value = value;
    }

    public static QuizId? TryCreate(int value)
    {
        if (value < 1)
            return null;
        return new QuizId(value);
    }
    
    public static QuizId Create(int value)
    {
        var quizId = TryCreate(value);
        if (quizId is null)
            throw new ArgumentException("Quiz id must be greater than 0", nameof(value));
        return new QuizId(value);
    }

    public bool Equals(QuizId? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((QuizId)obj);
    }

    public override int GetHashCode()
    {
        return Value;
    }
}