using System.Collections.Generic;
using System.Data.Common;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using QuizService.Model;
using QuizService.Model.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    public IActionResult Put(int id, [FromBody]QuizUpdateModel value)
    {
        const string sql = "UPDATE Quiz SET Title = @Title WHERE Id = @Id";
        int rowsUpdated = _connection.Execute(sql, new {Id = id, Title = value.Title});
        if (rowsUpdated == 0)
            return NotFound();
        return NoContent();
    }

    // DELETE api/quizzes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await new DeleteQuizCommand(_connection).Execute(QuizId.Create(id), cancellationToken);
        if (result.IsSuccess)
            return NoContent();
        return NotFound();
    }
}