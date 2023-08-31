using FluentValidation;
using QuizService.Application;

namespace QuizService.Model;

public class UpdateQuizRequest
{
    public string? Title { get; init; }
}

public class QuizUpdateRequestValidator : AbstractValidator<UpdateQuizRequest>
{
    public QuizUpdateRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .Must(title => QuizTitle.TryCreate(title) is not null);
    }
}