using System.Collections.Generic;
using System.Data.Common;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using QuizService.Model;
using QuizService.Model.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using QuizService.Application;
using Quiz = QuizService.Model.Domain.Quiz;

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
    [HttpGet("{id}")]
    public object Get(int id)
    {
        const string quizSql = "SELECT * FROM Quiz WHERE Id = @Id;";
        var quiz = _connection.QuerySingleOrDefault<Quiz>(quizSql, new {Id = id});
        if (quiz == null)
            return NotFound();
        const string questionsSql = "SELECT * FROM Question WHERE QuizId = @QuizId;";
        var questions = _connection.Query<Question>(questionsSql, new {QuizId = id});
        const string answersSql = "SELECT a.Id, a.Text, a.QuestionId FROM Answer a INNER JOIN Question q ON a.QuestionId = q.Id WHERE q.QuizId = @QuizId;";
        var answers = _connection.Query<Answer>(answersSql, new {QuizId = id})
            .Aggregate(new Dictionary<int, IList<Answer>>(), (dict, answer) => {
                if (!dict.ContainsKey(answer.QuestionId))
                    dict.Add(answer.QuestionId, new List<Answer>());
                dict[answer.QuestionId].Add(answer);
                return dict;
            });
        return new QuizResponseModel
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Questions = questions.Select(question => new QuizResponseModel.QuestionItem
            {
                Id = question.Id,
                Text = question.Text,
                Answers = answers.ContainsKey(question.Id)
                    ? answers[question.Id].Select(answer => new QuizResponseModel.AnswerItem
                    {
                        Id = answer.Id,
                        Text = answer.Text
                    })
                    : new QuizResponseModel.AnswerItem[0],
                CorrectAnswerId = question.CorrectAnswerId
            }),
            Links = new Dictionary<string, string>
            {
                {"self", $"/api/quizzes/{id}"},
                {"questions", $"/api/quizzes/{id}/questions"}
            }
        };
    }
    
    // PUT api/quizzes/5
    [HttpPut("{id}")]
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