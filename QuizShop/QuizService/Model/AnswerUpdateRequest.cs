using FluentValidation;
using QuizService.Application;

namespace QuizService.Model;

public class AnswerUpdateRequest
{
    public string? Text { get; init; }
}


public class AnswerUpdateRequestValidator : AbstractValidator<AnswerUpdateRequest>
{
    public AnswerUpdateRequestValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .Must(text => AnswerText.TryCreate(text, out _));
    }
}