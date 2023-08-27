using FluentValidation;
using QuizService.Application;

namespace QuizService.Model;

public class AddQuizRequest
{
    public string? Title { get; init; }
}

public class QuizModelValidator : AbstractValidator<AddQuizRequest>
{
    public QuizModelValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .Must(title => QuizTitle.TryCreate(title) is not null);
    }
}