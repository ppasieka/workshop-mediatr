using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace QuizService.Application;

internal record AddAnswerCommand(
    QuizId QuizId,
    QuestionId QuestionId,
    AnswerText AnswerText
);

internal class AddAnswerCommandHandler
{
    private readonly DbConnection _connection;
    
    public AddAnswerCommandHandler(DbConnection connection)
    {
        _connection = connection;
    }

    public async Task<Result<IError, AnswerId>> Execute(
        AddAnswerCommand command,
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
        var answerId = await _connection.ExecuteScalarAsync<long>(
            new CommandDefinition(
                commandText: "INSERT INTO Answer (Text, QuestionId) VALUES(@Text, @QuestionId); SELECT LAST_INSERT_ROWID();",
                parameters: new { Text = command.AnswerText, QuestionId = command.QuestionId.Value },
                transaction: transaction,
                cancellationToken: cancellationToken
            )
        );
        await transaction.CommitAsync(cancellationToken);
        
        return AnswerId.Create(answerId);
    }
}