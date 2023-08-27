using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace QuizService.Application;

internal class DeleteQuizCommandHandler
{
    private readonly DbConnection _connection;

    public DeleteQuizCommandHandler(DbConnection connection)
    {
        _connection = connection;
    }

    public async Task<VoidResult> Execute(QuizId quizId, CancellationToken cancellationToken)
    {
        if (!await SqlQuizExists.Execute(quizId, _connection, cancellationToken: cancellationToken))
            return new QuizNotFound(quizId: quizId);

        await using var transaction = await _connection.BeginTransactionAsync(cancellationToken: cancellationToken);
        await _connection.ExecuteAsync(command: new CommandDefinition(
                commandText: "DELETE FROM Quiz WHERE Id = @Id",
                parameters: new { Id = quizId.Value },
                cancellationToken: cancellationToken
            )
        );
        await transaction.CommitAsync(cancellationToken: cancellationToken);
        return default(ValueTuple);
    }
}