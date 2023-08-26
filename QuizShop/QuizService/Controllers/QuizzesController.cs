using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using Dapper;
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
    public IAsyncEnumerable<QuizResponseModel> Get(CancellationToken cancellationToken)
    {
        var query = new GetQuizzesQuery(_connection);
        return query.Execute(cancellationToken).Select(quiz =>
            new QuizResponseModel
            {
                Id = quiz.Id,
                Title = quiz.Title
            });
    }
    
    // POST api/quizzes
    [HttpPost]
    public IActionResult Post([FromBody]QuizCreateModel value)
    {
        var sql = $"INSERT INTO Quiz (Title) VALUES('{value.Title}'); SELECT LAST_INSERT_ROWID();";
        var id = _connection.ExecuteScalar(sql);
        return Created($"/api/quizzes/{id}", null);
    }

}