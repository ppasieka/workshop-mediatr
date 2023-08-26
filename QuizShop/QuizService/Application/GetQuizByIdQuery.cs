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
        var result = await _connection.QuerySingleAsync<QuizWithQuestions>(new CommandDefinition(
                commandText: sql,
                new { quizId },
                transaction: transaction,
                cancellationToken: cancellationToken
            )
        );
        return new QuizWithQuestions();
    }
}