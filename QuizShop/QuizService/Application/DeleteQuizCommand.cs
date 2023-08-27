using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace QuizService.Application;

using VoidResult = Result<IError, ValueTuple>;

internal class DeleteQuizCommand
{
    private readonly DbConnection _connection;

    public DeleteQuizCommand(DbConnection connection)
    {
        _connection = connection;
    }

    public async Task<VoidResult> Execute(QuizId quizId, CancellationToken cancellationToken)
    {
        var quiz = await _connection.QuerySingleOrDefaultAsync<Quiz>(
            command: new CommandDefinition(
                commandText: "SELECT * FROM Quiz WHERE Id = @Id;",
                parameters: new { Id = quizId.Value },
                cancellationToken: cancellationToken
            )
        );
        
        if (quiz == null)
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