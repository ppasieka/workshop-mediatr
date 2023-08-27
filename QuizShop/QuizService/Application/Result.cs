using System;

namespace QuizService.Application;

internal abstract class Result<TFailure, TSuccess> where TFailure : IError
{
    public abstract bool IsSuccess { get; }
    public abstract void WhenFailure(Action<TFailure> action);
    public abstract void WhenSuccess(Action<TSuccess> action);
    public abstract Result<TFailure, T1Right> Select<T1Right>(Func<TSuccess, T1Right> mapping);
    public abstract TResult Match<TResult>(Func<TFailure, TResult> failure, Func<TSuccess, TResult> success);

    public Result<TFailure, T1> Bind<T1>(Func<TSuccess, Result<TFailure, T1>> bindFunc) =>
        Match(
            failure: failure => new Failure<TFailure, T1>(failure),
            success: bindFunc
        );

    public abstract void Deconstruct(out bool isSuccess, out TFailure failure, out TSuccess success);


    public static implicit operator Result<TFailure, TSuccess>(TFailure failure) => new Failure<TFailure, TSuccess>(failure);
    public static implicit operator Result<TFailure, TSuccess>(TSuccess success) => new Success<TFailure, TSuccess>(success);
}


internal class Failure<TFailure, TSuccess> : Result<TFailure, TSuccess>
    where TFailure : IError
{
    private readonly TFailure _value;

    public override bool IsSuccess => false;

    public Failure(TFailure left)
    {
        _value = left;
    }

    public override void WhenFailure(Action<TFailure> action)
    {
        action(_value);
    }

    public override void WhenSuccess(Action<TSuccess> action)
    {
    }

    public override TResult Match<TResult>(Func<TFailure, TResult> failure, Func<TSuccess, TResult> success)
    {
        return failure(_value);
    }

    public override Result<TFailure, T1Right> Select<T1Right>(Func<TSuccess, T1Right> mapping)
    {
        return new Failure<TFailure, T1Right>(_value);
    }

    public override void Deconstruct(out bool isSuccess, out TFailure failure, out TSuccess success)
    {
        isSuccess = false;
        failure = _value;
        success = default!;
    }
}

internal class Success<TFailure, TSuccess> : Result<TFailure, TSuccess>
    where TFailure : IError
{
    private readonly TSuccess _value;
    public override bool IsSuccess => true;

    public Success(TSuccess value)
    {
        _value = value;
    }

    public override void WhenFailure(Action<TFailure> action)
    {
    }

    public override void WhenSuccess(Action<TSuccess> action)
    {
        action(_value);
    }

    public override Result<TFailure, T1Right> Select<T1Right>(Func<TSuccess, T1Right> mapping)
    {
        return new Success<TFailure, T1Right>(mapping(_value));
    }

    public override TResult Match<TResult>(Func<TFailure, TResult> failure, Func<TSuccess, TResult> success)
    {
        return success(_value);
    }

    public override void Deconstruct(out bool isSuccess, out TFailure failure, out TSuccess success)
    {
        isSuccess = true;
        failure = default!;
        success = _value;
    }
}
