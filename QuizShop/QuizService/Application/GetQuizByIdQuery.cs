using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace QuizService.Application;

internal class GetQuizByIdQuery
{
    private readonly DbConnection _connection;

    public GetQuizByIdQuery(DbConnection connection)
    {
        _connection = connection;
    }
    public async Task<QuizWithQuestions?> Execute(QuizId quizId, CancellationToken cancellationToken)
    {
        await using var transaction = await _connection.BeginTransactionAsync(cancellationToken);
        // Query 1: Fetch the quiz
        var quiz = await _connection.QuerySingleOrDefaultAsync<QuizWithQuestions>(
            new CommandDefinition(
                commandText: @"
SELECT Id, Title FROM Quiz WHERE Id = @QuizId",
                parameters: new { QuizId = quizId.Value },
                transaction: transaction,
                cancellationToken: cancellationToken
            )
        );
        if (quiz == null)
            return null;

        // Query 2: Fetch questions for the quiz
        var questions = await _connection.QueryAsync<Question>(
            new CommandDefinition(
                commandText: @"
SELECT Id, Text, CorrectAnswerId FROM Question WHERE QuizId = @QuizId",
                parameters: new { QuizId = quizId.Value },
                transaction: transaction,
                cancellationToken: cancellationToken
            )
        );
        quiz.Questions.Add(questions);
        
        // Query 3: Fetch answers for the questions
        var answers = await _connection.QueryAsync<Answer>(
            new CommandDefinition(
                commandText: @"
SELECT Answer.Id, Answer.QuestionId, Answer.Text 
FROM Answer 
JOIN Question ON Answer.QuestionId = Question.Id 
WHERE Question.QuizId = @QuizId",
                parameters: new { QuizId = quizId.Value },
                transaction: transaction,
                cancellationToken: cancellationToken
            )
        );
        foreach (var answer in answers)
        {
            quiz.Questions.TryAddAnswer(answer);
        }

        return quiz;
    }
}