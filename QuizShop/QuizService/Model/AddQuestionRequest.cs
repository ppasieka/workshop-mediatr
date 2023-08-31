using FluentValidation;
using QuizService.Application;

namespace QuizService.Model;

public class AddQuestionRequest
{
    public AddQuestionRequest(string text)
    {
        Text = text;
    }

    public string Text { get; set; }
}

public class AddQuestionRequestValidator : AbstractValidator<AddQuestionRequest>
{
    public AddQuestionRequestValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .Must(text => QuestionText.TryCreate(text, out _));
    }
}