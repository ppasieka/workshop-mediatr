using System;
using System.Text.RegularExpressions;
using Dapper;

namespace QuizService.Application;

internal class QuizTitle : IEquatable<QuizTitle>
{
    public string Value { get; }

    private QuizTitle(string value) => Value = value;

    public bool Equals(QuizTitle? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((QuizTitle) obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(QuizTitle? left, QuizTitle? right) => Equals(left, right);

    public static bool operator !=(QuizTitle? left, QuizTitle? right) => !Equals(left, right);

    public static implicit operator string(QuizTitle quizTitle) => quizTitle.Value;
    public static QuizTitle? TryCreate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        if (!Regex.IsMatch(value, @"^[a-zA-Z0-9 ]+$"))
            return null;
        return new QuizTitle(value);
    }
    
    public static QuizTitle Create(string? value)
    {
        var quizTitle = TryCreate(value);
        if (quizTitle is null)
            throw new ArgumentException($"Invalid quiz title: {value}", nameof(value));
        return quizTitle;
    }
}

internal class QuizTitleTypeHandler : SqlMapper.TypeHandler<QuizTitle>
{
    public override QuizTitle Parse(object value)
    {
        return QuizTitle.Create((string)value);
    }

    public override void SetValue(System.Data.IDbDataParameter parameter, QuizTitle value)
    {
        parameter.Value = value.Value;
    }
}