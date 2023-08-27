using System;
using Dapper;

namespace QuizService.Application;

internal class AnswerId : IEquatable<AnswerId>
{
    public long Value { get; }
    
    private AnswerId(long value) => Value = value;

    public static AnswerId? TryCreate(long value)
    {
        if (value < 1)
            return null;
        return new AnswerId(value);
    }
    
    public static AnswerId Create(long value)
    {
        var answerId = TryCreate(value);
        if (answerId is null)
            throw new ArgumentException("Answer id must be greater than 0", nameof(value));
        return new AnswerId(value);
    }

    public bool Equals(AnswerId? other)
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
        return Equals((AnswerId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();
    public static implicit operator long(AnswerId answerId) => answerId.Value;
}

internal class AnswerIdTypeHandler : SqlMapper.TypeHandler<AnswerId>
{
    public override AnswerId Parse(object value)
    {
        return AnswerId.Create((long)value);
    }

    public override void SetValue(System.Data.IDbDataParameter parameter, AnswerId value)
    {
        parameter.Value = value.Value;
    }
}