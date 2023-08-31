using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using QuizService.Model;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using QuizService.Application;

namespace QuizService.Controllers;

[ApiController]
[Route("api/quizzes")]
public class QuizController : ControllerBase
{
    private readonly DbConnection _connection;

    public QuizController(DbConnection connection)
    {
        _connection = connection;
    }

    // GET api/quizzes/5
    [HttpGet("{id:long}")]
    public async Task<object> Get(long id, CancellationToken cancellationToken)
    {
        if (!QuizId.TryCreate(id, out var quizId))
        {
            ModelState.AddModelError("id", "Invalid quiz id");
            return BadRequest();
        }

        var quiz = await new GetQuizByIdQuery(_connection).Execute(quizId, cancellationToken);
        if (quiz == null)
            return NotFound();
        
        return new QuizResponse
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Questions = quiz.Questions.Select(question => new QuestionItemResponse
            {
                Id = question.Id,
                Text = question.Text,
                Answers = question
                        .Answers
                        .Select(answer => new AnswerItemResponse
                    {
                        Id = answer.Id,
                        Text = answer.Text
                    }),
                CorrectAnswerId = question.CorrectAnswerId?.Value
            }),
            Links = new Dictionary<string, string>
            {
                {"self", $"/api/quizzes/{id}"},
                {"questions", $"/api/quizzes/{id}/questions"}
            }
        };
    }
    
    // PUT api/quizzes/5
    [HttpPut("{id:}")]
    public async Task<IActionResult> Put(
        [FromRoute] int id,
        [FromBody] UpdateQuizRequest value,
        [FromServices] IValidator<UpdateQuizRequest> validator,
        CancellationToken cancellationToken
    )
    {
        ValidationResult result = validator.Validate(value);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }
        var (isSuccess, failure, _) = await new UpdateQuizTitleCommandHandler(_connection).Execute(
            new UpdateQuizTitleCommand(
                QuizId.Create(id),
                QuizTitle.Create(value.Title)
            ),
            cancellationToken);
        if (isSuccess)
            return NoContent();
        if (failure is QuizNotFound)
            return NotFound();
        ModelState.AddModelError("error", failure.Message);
        return BadRequest();
    }

    // DELETE api/quizzes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await new DeleteQuizCommandHandler(_connection).Execute(QuizId.Create(id), cancellationToken);
        if (result.IsSuccess)
            return NoContent();
        return NotFound();
    }
}