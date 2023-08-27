namespace QuizService.Application;

internal interface IError
{
    string Message { get; }
    string Code { get; }
}

internal class QuizNotFound : IError
{
    public QuizNotFound(QuizId quizId)
    {
        Message = $"Quiz with id {quizId.Value} not found";
        Code = "quiz_not_found";
    }

    public string Message { get; }
    public string Code { get; }
}

