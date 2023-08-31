using FluentValidation;
using QuizService.Application;

namespace QuizService.Model;

public class QuestionUpdateRequest
{
    public string? Text { get; init; }
    public long? CorrectAnswerId { get; init; }
}

public class QuestionUpdateRequestValidator : AbstractValidator<QuestionUpdateRequest>
{
    public QuestionUpdateRequestValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .Must(text => QuestionText.TryCreate(text, out _));
        When(x => x.CorrectAnswerId is not null, () =>
        {
            RuleFor(x => x.CorrectAnswerId)
                .Must(id => AnswerId.TryCreate(id.Value, out _));
        });
    }
}