using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace QuizService.Application;

internal record UpdateQuizTitleCommand(QuizId QuizId, QuizTitle Title);

internal class UpdateQuizTitleCommandHandler
{
    private readonly DbConnection _connection;
    public UpdateQuizTitleCommandHandler(DbConnection connection)
    {
        _connection = connection;
    }

    public async Task<UnitResult> Execute(
        UpdateQuizTitleCommand command,
        CancellationToken cancellationToken
    )
    {
        if (!await SqlQuizExists.Execute(command.QuizId, _connection, cancellationToken: cancellationToken))
            return new QuizNotFound(command.QuizId);

        var transaction = await _connection.BeginTransactionAsync(cancellationToken);
        await _connection.ExecuteAsync(
            new CommandDefinition(
                commandText: "UPDATE Quiz SET Title = @Title WHERE Id = @Id",
                parameters: new { Id = command.QuizId.Value, Title = command.Title.Value },
                transaction: transaction,
                cancellationToken: cancellationToken)
            );
        await transaction.CommitAsync(cancellationToken);
        return Unit.Instance;
    }
}