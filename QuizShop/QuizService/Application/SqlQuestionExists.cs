using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace QuizService.Application;

internal static class SqlQuestionExists
{
    public static async Task<bool> Execute(
        QuizId quizId,
        QuestionId questionId,
        DbConnection connection,
        DbTransaction? transaction = null,
        CancellationToken cancellationToken = default
    )
    {
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                commandText: "SELECT COUNT(*) FROM Question WHERE Id = @QuestionId AND QuizId = @QuizId;",
                new { QuestionId = questionId.Value, QuizId = quizId.Value },
                transaction: transaction,
                cancellationToken: cancellationToken
            )
        ) > 0;
    }
}