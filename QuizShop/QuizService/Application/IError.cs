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

internal class QuizAlreadyExists : IError
{
    public QuizAlreadyExists(QuizTitle quizTitle)
    {
        Message = $"Quiz with title {quizTitle.Value} already exists";
        Code = "quiz_already_exists";
    }

    public string Message { get; }
    public string Code { get; }
}