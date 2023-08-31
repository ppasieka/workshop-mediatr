using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace QuizService.Application;

internal record DeleteQuestionCommand(QuizId QuizId, QuestionId QuestionId);
internal class DeleteQuestionCommandHandler
{
    private readonly DbConnection _connection;
    
    public DeleteQuestionCommandHandler(DbConnection connection)
    {
        _connection = connection;
    }

    public async Task<UnitResult> Execute(
        DeleteQuestionCommand command,
        CancellationToken cancellationToken
    )
    {
        var questionExists = await SqlQuestionExists.Execute(
            command.QuizId,
            command.QuestionId,
            _connection,
            cancellationToken: cancellationToken
        );
        if (!questionExists)
            return new QuestionNotFound(command.QuizId, command.QuestionId);
        
        await using var transaction = await _connection.BeginTransactionAsync(cancellationToken: cancellationToken);
        await _connection.ExecuteAsync(
            new CommandDefinition(
                commandText: "DELETE FROM Question WHERE Id = @Id AND QuizId = @QuizId",
                parameters: new { Id = command.QuestionId.Value, QuizId = command.QuizId.Value },
                transaction: transaction,
                cancellationToken: cancellationToken
            )
        );
        await transaction.CommitAsync(cancellationToken);
        
        return Unit.Instance;
    }
    
}