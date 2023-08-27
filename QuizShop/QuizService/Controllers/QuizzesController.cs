using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using QuizService.Application;
using QuizService.Model;

namespace QuizService.Controllers;

[ApiController]
[Route("api/quizzes")]
public class QuizzesController : ControllerBase
{

    private readonly DbConnection _connection;
    public QuizzesController(DbConnection connection)
    {
        _connection = connection;
    }
    
    // GET api/quizzes
    [HttpGet]
    public IAsyncEnumerable<QuizItemResponse> Get(CancellationToken cancellationToken)
    {
        var query = new GetQuizzesQuery(_connection);
        return query.Execute(cancellationToken).Select(quiz =>
            new QuizItemResponse
            {
                Id = quiz.Id,
                Title = quiz.Title
            });
    }
    
    // POST api/quizzes
    [HttpPost]
    public async Task<IActionResult> Post(
        [FromBody] AddQuizRequest value,
        [FromServices] IValidator<AddQuizRequest> validator,
        CancellationToken cancellationToken
    )
    {
        var result = validator.Validate(value);
        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        var (isSuccess, failure, quizId) = await new AddQuizCommandHandler(_connection).Execute(
                new AddQuizCommand(QuizTitle.Create(value.Title)),
                cancellationToken
            );
        if (!isSuccess)
        {
            ModelState.AddModelError("error", failure.Message);
            return BadRequest(ModelState);
        }
        return Created($"/api/quizzes/{quizId.Value}", null);
    }
}