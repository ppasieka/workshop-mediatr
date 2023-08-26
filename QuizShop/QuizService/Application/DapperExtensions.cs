using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading;
using Dapper;

namespace QuizService.Application;

public static class DapperExtensions
{
    /// <summary>
    /// Asynchronously enumerates the results of a query.
    /// </summary>
    /// <typeparam name="T">The type of result to return.</typeparam>
    /// <param name="connection">The connection to query on.</param>
    /// <param name="sql">The SQL to execute for the query.</param>
    /// <param name="param">The parameters to pass, if any.</param>
    /// <param name="transaction">The transaction to use, if any.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>An asynchronous enumerator of the results.</returns>
    /// <remarks>See <see href="https://stackoverflow.com/a/66723553/1178314"/> and
    /// <see href="https://github.com/DapperLib/Dapper/issues/1239#issuecomment-1035507322"/>.</remarks>
    public static async IAsyncEnumerable<T> EnumerateAsync<T>(
        this DbConnection connection,
        string sql,
        object? param = null,
        IDbTransaction? transaction = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        await using var reader = await connection.ExecuteReaderAsync(sql, param, transaction);
        var rowParser = reader.GetRowParser<T>();
        while (await reader.ReadAsync(cancellationToken))
        {
            yield return rowParser(reader);
        }
        while (await reader.NextResultAsync(cancellationToken)) { }
    } 
    
}