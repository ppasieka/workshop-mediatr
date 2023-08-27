using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace QuizService.Application;

internal class GetQuizByIdQuery
{
    private readonly DbConnection _connection;

    public GetQuizByIdQuery(DbConnection connection)
    {
        _connection = connection;
    }


    public async Task<QuizWithQuestions> GetById(QuizId quizId, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM Quiz WHERE Id = @Id";
        // using dapper query for a quiz with questions and answers
        await using var transaction = await _connection.BeginTransactionAsync(cancellationToken);
        var quizQuery = "SELECT * FROM Quiz WHERE Id = @QuizId";
        quiz = db.QueryFirstOrDefault<Quiz>(quizQuery, new { QuizId = quizId });

        if (quiz == null)
            return null;

        quiz.Questions = new List<Question>();

        // Query 2: Fetch questions for the quiz
        var questionQuery = "SELECT * FROM Question WHERE QuizId = @QuizId";
        var questions = db.Query<Question>(questionQuery, new { QuizId = quizId }).ToList();

        // Query 3: Fetch answers for the questions
        var answerQuery = "SELECT * FROM Answer WHERE QuestionId IN @QuestionIds";
        var answers = db.Query<Answer>(answerQuery, new { QuestionIds = questions.Select(q => q.Id) }).ToList();

        // Build the result
        foreach (var question in questions)
        {
            question.Answers = answers.Where(a => a.QuestionId == question.Id).ToList();
            quiz.Questions.Add(question);
        }        return new QuizWithQuestions();
    }
}