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

internal class QuestionNotFound : IError
{
    public QuestionNotFound(QuizId quizId, QuestionId questionId)
    {
        Message = $"Question with id {questionId.Value} not found in quiz with id {quizId.Value}";
        Code = "question_not_found";
    }

    public string Message { get; }
    public string Code { get; }
}

internal class CorrectAnswerNotFound : IError
{
    public CorrectAnswerNotFound(AnswerId answerId)
    {
        Message = $"Correct answer with id {answerId.Value} not found";
        Code = "correct_answer_not_found";
    }

    public string Message { get; }
    public string Code { get; }
}

internal class AnswerNotFound : IError
{
    public AnswerNotFound(QuestionId questionId, AnswerId answerId)
    {
        Message = $"Answer with id {answerId.Value} not found in question with id {questionId.Value}";
        Code = "answer_not_found";
    }

    public string Message { get; }
    public string Code { get; }
}