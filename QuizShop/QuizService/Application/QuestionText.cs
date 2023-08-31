using System;
using System.Diagnostics.CodeAnalysis;
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
    public static bool TryCreate(string? value, [NotNullWhen(true)] out QuestionText? questionText)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 255)
        {
            questionText = null;
            return false;
        }

        questionText = new QuestionText(value.Trim());
        return true;
    }
    
    public static QuestionText Create(string? value)
    {
        if (!TryCreate(value, out var questionText))
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
