using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading;

namespace QuizService.Application;

internal class GetQuizzesQuery
{
    private readonly DbConnection _connection;

    public GetQuizzesQuery(DbConnection connection)
    {
        _connection = connection;
    }
    public async IAsyncEnumerable<Quiz> Execute([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using var transaction = await _connection.BeginTransactionAsync(cancellationToken);

        await foreach (var quiz in _connection.EnumerateAsync<Quiz>(
                           "SELECT * FROM Quiz",
                           transaction: transaction,
                           cancellationToken: cancellationToken
                       )
        )
            yield return quiz;
        await transaction.CommitAsync(cancellationToken);
    }
}