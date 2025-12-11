using Domain.Abstraction.Errors;

namespace Domain.Abstraction.Results;

public class Result
{
    protected internal Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("Success result cannot have an error.");
        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("Failure result must have an error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static Result Failure(string code, string message) =>
        new(false, Error.Custom(code, message));
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value
    {
        get
        {
            if (!IsSuccess)
                throw new InvalidOperationException("Cannot access Value of a failed result.");

            return _value!;
        }
    }

    public static Result<TValue> Success(TValue value) =>
        new(value, true, Error.None);

    public static new Result<TValue> Failure(Error error) =>
        new(default, false, error);

    public new static Result<TValue> Failure(string code, string message) =>
        new(default, false, Error.Custom(code, message));

    public static Result<TValue> Create(TValue? value) =>
        value is not null ? Success(value) : Failure(Error.NullValue);

    public static implicit operator Result<TValue>(TValue? value) => Create(value);
}
