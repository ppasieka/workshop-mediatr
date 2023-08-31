using System;
using Dapper;

namespace QuizService.Application;

internal class QuestionId : IEquatable<QuestionId>
{
    public long Value { get; }
    
    private QuestionId(long value) => Value = value;

    public static QuestionId? TryCreate(long value)
    {
        if (value < 1)
            return null;
        return new QuestionId(value);
    }
    
    public static QuestionId Create(long value)
    {
        var questionId = TryCreate(value);
        if (questionId is null)
            throw new ArgumentException("Question id must be greater than 0", nameof(value));
        return new QuestionId(value);
    }

    public bool Equals(QuestionId? other)
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
        return Equals((QuestionId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();
    public static implicit operator long(QuestionId questionId) => questionId.Value;
}

internal class QuestionIdTypeHandler : SqlMapper.TypeHandler<QuestionId>
{
    public override QuestionId Parse(object value)
    {
        return QuestionId.Create((long)value);
    }

    public override void SetValue(System.Data.IDbDataParameter parameter, QuestionId value)
    {
        parameter.Value = value.Value;
    }
}