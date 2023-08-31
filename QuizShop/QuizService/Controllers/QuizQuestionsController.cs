using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using QuizService.Application;
using QuizService.Model;

namespace QuizService.Controllers;

[ApiController]
[Route("api/quizzes")]
public class QuizQuestionsController : ControllerBase
{
    private readonly DbConnection _connection;

    public QuizQuestionsController(DbConnection connection)
    {
        _connection = connection;
    }

    // POST api/quizzes/5/questions
    [HttpPost]
    [Route("{id:long}/questions")]
    public async Task<IActionResult> PostQuestion(
        [FromRoute] long id,
        [FromBody] AddQuestionRequest value,
        [FromServices] IValidator<AddQuestionRequest> validator,
        CancellationToken cancellationToken
    )
    {
        var validationResult = validator.Validate(value);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }
        var command = new AddQuestionCommand(QuizId.Create(id), QuestionText.Create(value.Text));
        var (isSuccess, failure, questionId) = await new AddQuestionCommandHandler(_connection).Execute(command, cancellationToken);

        if (!isSuccess)
        {
            if (failure is QuizNotFound)
                return NotFound();
            
            ModelState.AddModelError("error", failure.Message);
            return BadRequest(ModelState);
        }
        
        return Created($"/api/quizzes/{id}/questions/{questionId}", null);
    }

    // PUT api/quizzes/5/questions/6
    [HttpPut("{id:long}/questions/{qid:long}")]
    public async Task<IActionResult> PutQuestion(
        [FromRoute] long id,
        [FromRoute] long qid,
        [FromBody] QuestionUpdateRequest value,
        [FromServices] IValidator<QuestionUpdateRequest> validator,
        CancellationToken cancellationToken
    )
    {
        var validationResult = validator.Validate(value);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }
        
        var command = new UpdateQuestionCommand(
            QuizId.Create(id),
            QuestionId.Create(qid),
            QuestionText.Create(value.Text),
            value.CorrectAnswerId is null ? null : AnswerId.Create(value.CorrectAnswerId.Value)
        );
        var (isSuccess, _, _) = await new UpdateQuestionCommandHandler(_connection).Execute(command, cancellationToken);
        // TODO: consider returning 400 when command failed
        
        if (!isSuccess)
            return NotFound();
        return NoContent();
    }

    // DELETE api/quizzes/5/questions/6
    [HttpDelete]
    [Route("{id}/questions/{qid}")]
    public IActionResult DeleteQuestion(int id, int qid)
    {
        const string sql = "DELETE FROM Question WHERE Id = @QuestionId";
        _connection.ExecuteScalar(sql, new {QuestionId = qid});
        return NoContent();
    }

    // POST api/quizzes/5/questions/6/answers
    [HttpPost]
    [Route("{id}/questions/{qid}/answers")]
    public IActionResult PostAnswer(int id, int qid, [FromBody]AnswerCreateModel value)
    {
        const string sql = "INSERT INTO Answer (Text, QuestionId) VALUES(@Text, @QuestionId); SELECT LAST_INSERT_ROWID();";
        var answerId = _connection.ExecuteScalar(sql, new {Text = value.Text, QuestionId = qid});
        return Created($"/api/quizzes/{id}/questions/{qid}/answers/{answerId}", null);
    }

    // PUT api/quizzes/5/questions/6/answers/7
    [HttpPut("{id}/questions/{qid}/answers/{aid}")]
    public IActionResult PutAnswer(int id, int qid, int aid, [FromBody]AnswerUpdateModel value)
    {
        const string sql = "UPDATE Answer SET Text = @Text WHERE Id = @AnswerId";
        int rowsUpdated = _connection.Execute(sql, new {AnswerId = qid, Text = value.Text});
        if (rowsUpdated == 0)
            return NotFound();
        return NoContent();
    }

    // DELETE api/quizzes/5/questions/6/answers/7
    [HttpDelete]
    [Route("{id}/questions/{qid}/answers/{aid}")]
    public IActionResult DeleteAnswer(int id, int qid, int aid)
    {
        const string sql = "DELETE FROM Answer WHERE Id = @AnswerId";
        _connection.ExecuteScalar(sql, new {AnswerId = aid});
        return NoContent();
    }
}