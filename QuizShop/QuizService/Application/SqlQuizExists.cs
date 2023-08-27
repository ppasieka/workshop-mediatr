using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace QuizService.Application;

internal static class SqlQuizExists
{
    public static async Task<bool> Execute(
        QuizId quizId,
        DbConnection connection,
        DbTransaction? transaction = null,
        CancellationToken cancellationToken = default
    )
    {
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                commandText: "SELECT COUNT(*) FROM Quiz WHERE Id = @Id;",
                new { Id = quizId.Value },
                transaction: transaction,
                cancellationToken: cancellationToken
            )
        ) > 0;
    }

    public static async Task<bool> Execute(
        QuizTitle quizTitle,
        DbConnection connection,
        DbTransaction? transaction = null,
        CancellationToken cancellationToken = default
    )
    {
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                commandText: "SELECT COUNT(*) FROM Quiz WHERE Title = @Title;",
                new { Title = quizTitle.Value },
                transaction: transaction,
                cancellationToken: cancellationToken
            )
        ) > 0;
    }
}