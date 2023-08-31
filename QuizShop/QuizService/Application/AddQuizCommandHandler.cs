using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace QuizService.Application;

internal record AddQuizCommand(QuizTitle Title);

internal class AddQuizCommandHandler
{
    private readonly DbConnection _connection;
    public AddQuizCommandHandler(DbConnection connection)
    {
        _connection = connection;
    }

    public async Task<Result<IError, QuizId>> Execute(
        AddQuizCommand command,
        CancellationToken cancellationToken
    )
    {
        bool quizExist = await SqlQuizExists.Execute(command.Title, _connection, cancellationToken: cancellationToken);
        if (quizExist)
            return new QuizAlreadyExists(command.Title);
        return await AddQuiz(command, cancellationToken);
    }

    private async Task<QuizId> AddQuiz(
        AddQuizCommand command,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _connection.BeginTransactionAsync(cancellationToken);
        var quizId = await _connection.ExecuteScalarAsync<QuizId>(
            new CommandDefinition(
                commandText: "INSERT INTO Quiz (Title) VALUES (@Title); SELECT LAST_INSERT_ROWID();",
                parameters: new { Title = command.Title.Value },
                transaction: transaction,
                cancellationToken: cancellationToken
            )
        );
        await transaction.CommitAsync(cancellationToken);
        return QuizId.Create(quizId);
    }
}