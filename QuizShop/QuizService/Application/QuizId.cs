using System;
using Dapper;

namespace QuizService.Application;

internal class QuizId : IEquatable<QuizId>
{
    public long Value { get; }
    
    private QuizId(long value) => Value = value;

    public static QuizId? TryCreate(long value)
    {
        if (value < 1)
            return null;
        return new QuizId(value);
    }
    
    public static QuizId Create(long value)
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

    public override int GetHashCode() => Value.GetHashCode();
    public static implicit operator long(QuizId quizId) => quizId.Value;
}

internal class QuizIdTypeHandler : SqlMapper.TypeHandler<QuizId>
{
    public override QuizId Parse(object value)
    {
        return QuizId.Create((long)value);
    }

    public override void SetValue(System.Data.IDbDataParameter parameter, QuizId value)
    {
        parameter.Value = value.Value;
    }
}