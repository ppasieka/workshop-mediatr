using System;
using System.Diagnostics.CodeAnalysis;
using Dapper;

namespace QuizService.Application;

internal class AnswerText : IEquatable<AnswerText>
{
    public string Value { get; }

    private AnswerText(string value) => Value = value;

    public bool Equals(AnswerText? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((AnswerText) obj);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(AnswerText? left, AnswerText? right) => Equals(left, right);

    public static bool operator !=(AnswerText? left, AnswerText? right) => !Equals(left, right);

    public static implicit operator string(AnswerText answerText) => answerText.Value;
    public static bool TryCreate(string? value, [NotNullWhen(true)] out AnswerText? answerText)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 255)
        {
            answerText = null;
            return false;
        }
        answerText = new AnswerText(value.Trim());
        return true;
    }
    
    public static AnswerText Create(string? value)
    {
        if (!TryCreate(value, out var answerText))
            throw new ArgumentException($"Invalid answer text: {value}", nameof(value));
        return answerText;
    }
}

internal class AnswerTextTypeHandler : SqlMapper.TypeHandler<AnswerText>
{
    public override AnswerText Parse(object value)
    {
        return AnswerText.Create((string)value);
    }

    public override void SetValue(System.Data.IDbDataParameter parameter, AnswerText value)
    {
        parameter.Value = value.Value;
    }
}