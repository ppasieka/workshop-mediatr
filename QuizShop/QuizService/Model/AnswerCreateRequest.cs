using FluentValidation;
using QuizService.Application;

namespace QuizService.Model;

public class AnswerCreateRequest
{
    public string? Text { get; init; }
}

public class AnswerCreateRequestValidator : AbstractValidator<AnswerCreateRequest>
{
    public AnswerCreateRequestValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Text is required")
            .Must(text => AnswerText.TryCreate(text, out _));
    }
}