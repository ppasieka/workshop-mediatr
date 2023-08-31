using System;
using System.Diagnostics.CodeAnalysis;
using Dapper;

namespace QuizService.Application;

internal class AnswerId : IEquatable<AnswerId>
{
    public long Value { get; }
    
    private AnswerId(long value) => Value = value;

    public static bool TryCreate(long value, [NotNullWhen(true)] out AnswerId? answerId)
    {
        if (value < 1)
        {
            answerId = null;
            return false;
        }

        answerId = new AnswerId(value);
        return true;
    }
    
    public static AnswerId Create(long value)
    {
        if (!TryCreate(value, out var answerId))
            throw new ArgumentException("Answer id must be greater than 0", nameof(value));
        return answerId;
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