using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace QuizService.Application;

internal record UpdateQuestionCommand(
    QuizId QuizId,
    QuestionId QuestionId,
    QuestionText QuestionText,
    AnswerId? CorrectAnswerId
);

internal class UpdateQuestionCommandHandler
{
    private readonly DbConnection _connection;
    public UpdateQuestionCommandHandler(DbConnection connection)
    {
        _connection = connection;
    }
    public async Task<UnitResult> Execute(
        UpdateQuestionCommand command,
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

        if (command.CorrectAnswerId is not null)
        {
            var answerExists = await SqlAnswerExists.Execute(
                command.QuestionId,
                command.CorrectAnswerId,
                connection: _connection,
                cancellationToken: cancellationToken
            );
            if (!answerExists)
                return new AnswerNotFound(command.QuestionId, command.CorrectAnswerId);
        }
        
        var transaction = await _connection.BeginTransactionAsync(cancellationToken);
        await _connection.ExecuteAsync(
            new CommandDefinition(
                commandText: "UPDATE Question SET Text = @Text, CorrectAnswerId = @CorrectAnswerId WHERE Id = @Id",
                parameters: new
                {
                    Id = command.QuestionId.Value,
                    Text = command.QuestionText.Value,
                    CorrectAnswerId = command.CorrectAnswerId?.Value
                },
                transaction: transaction,
                cancellationToken: cancellationToken)
        );
        
        return Unit.Instance;
    }
}