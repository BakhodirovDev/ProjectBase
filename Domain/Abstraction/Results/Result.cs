using Domain.Abstraction.Errors;
using System.Text.Json.Serialization;

namespace Domain.Abstraction.Results;

public class Result
{
    private readonly Error _error;
    private readonly string? _message;

    protected internal Result(bool isSuccess, Error? error, string? message = null)
    {
        if (isSuccess && error != null && error != Error.None)
            throw new InvalidOperationException("Success result cannot have an error.");

        if (!isSuccess && (error == null || error == Error.None))
            throw new InvalidOperationException("Failure result must have an error.");

        IsSuccess = isSuccess;
        _error = error ?? Error.None;
        _message = message;
    }

    public bool IsSuccess { get; }

    public string Message => _message ?? _error?.Message ?? (IsSuccess ? "Successfully" : "An error occurred");
    
    [JsonIgnore]
    public Error? Error => IsSuccess ? null : _error;

    public static Result Success(string? message = null) => new(true, Error.None, message);

    public static Result Failure(Error error) => new(false, error);

    public static Result Failure(string code, string message) =>
        new(false, Error.Custom(code, message));

    public static Result SuccessWithMessage(string message) => new(true, Error.None, message);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, Error? error, string? message = null)
        : base(isSuccess, error, message)
    {
        _value = value;
    }

    [JsonIgnore]
    public TValue Value
    {
        get
        {
            if (!IsSuccess)
                throw new InvalidOperationException("Cannot access Value of a failed result.");
            return _value!;
        }
    }

    public TValue? Data => IsSuccess ? _value : default;

    public static Result<TValue> Success(TValue value, string? message = null) =>
        new(value, true, Error.None, message);

    public static new Result<TValue> Failure(Error? error) =>
        new(default, false, error);

    public new static Result<TValue> Failure(string code, string message) =>
        new(default, false, Error.Custom(code, message));

    public static Result<TValue> Create(TValue? value) =>
        value is not null ? Success(value) : Failure(Error.NullValue);

    public static Result<TValue> SuccessWithMessage(TValue value, string message) =>
        new(value, true, Error.None, message);

    public static implicit operator Result<TValue>(TValue? value) => Create(value);
}