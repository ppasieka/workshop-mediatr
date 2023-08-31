using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace QuizService.Application;

internal static class SqlAnswerExists
{
    public static async Task<bool> Execute(
        QuestionId questionId,
        AnswerId answerId,
        DbConnection connection,
        DbTransaction? transaction = null,
        CancellationToken cancellationToken = default
    )
    {
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                commandText: "SELECT COUNT(*) FROM Answer WHERE Id = @AnswerId AND QuestionId = @QuestionId;",
                new { AnswerId = answerId.Value, QuestionId = questionId.Value },
                transaction: transaction,
                cancellationToken: cancellationToken
            )
        ) > 0;
    }
}