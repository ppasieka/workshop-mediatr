using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace QuizService.Application;

internal record AddQuestionCommand(QuizId QuizId, QuestionText QuestionText);

internal class AddQuestionCommandHandler
{
    private readonly DbConnection _connection;

    public AddQuestionCommandHandler(DbConnection connection)
    {
        _connection = connection;
    }

    public async Task<Result<IError, QuestionId>> Execute(
        AddQuestionCommand command,
        CancellationToken cancellationToken
    )
    {
        var quizExists = await SqlQuizExists.Execute(command.QuizId, _connection, cancellationToken: cancellationToken);
        if (!quizExists)
            return new QuizNotFound(command.QuizId);

        await using var transaction = await _connection.BeginTransactionAsync(cancellationToken: cancellationToken);
        var questionId = await _connection.ExecuteScalarAsync<long>(
            new CommandDefinition(
                commandText: "INSERT INTO Question (Text, QuizId) VALUES(@Text, @QuizId); SELECT LAST_INSERT_ROWID();",
                parameters: new { Text = command.QuestionText, QuizId = command.QuizId.Value },
                transaction: transaction,
                cancellationToken: cancellationToken
            )
        );

        return QuestionId.Create(questionId);
    }
}