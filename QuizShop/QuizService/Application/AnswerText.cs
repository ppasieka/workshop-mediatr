using System;
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
    public static AnswerText? TryCreate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        if (value.Length > 255)
            return null;
        
        return new AnswerText(value.Trim());
    }
    
    public static AnswerText Create(string? value)
    {
        var answerText = TryCreate(value);
        if (answerText is null)
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