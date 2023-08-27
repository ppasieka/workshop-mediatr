using System;
using Dapper;

namespace QuizService.Application;

internal class QuestionText : IEquatable<QuestionText>
{
    public string Value { get; }

    private QuestionText(string value) => Value = value;

    public bool Equals(QuestionText? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((QuestionText) obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(QuestionText? left, QuestionText? right) => Equals(left, right);

    public static bool operator !=(QuestionText? left, QuestionText? right) => !Equals(left, right);

    public static implicit operator string(QuestionText questionText) => questionText.Value;
    public static QuestionText? TryCreate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        if (value.Length > 255)
            return null;
        
        return new QuestionText(value.Trim());
    }
    
    public static QuestionText Create(string? value)
    {
        var questionText = TryCreate(value);
        if (questionText is null)
            throw new ArgumentException($"Invalid question text: {value}", nameof(value));
        return questionText;
    }
}

internal class QuestionTextTypeHandler : SqlMapper.TypeHandler<QuestionText>
{
    public override QuestionText Parse(object value)
    {
        return QuestionText.Create((string)value);
    }

    public override void SetValue(System.Data.IDbDataParameter parameter, QuestionText value)
    {
        parameter.Value = value.Value;
    }
}
