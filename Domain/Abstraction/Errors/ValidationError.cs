using Domain.Abstraction.Results;

namespace Domain.Abstraction.Errors;

public sealed class ValidationError
{
    public Dictionary<string, string[]> Errors { get; }
    public Error Error { get; }

    private ValidationError(Dictionary<string, string[]> errors)
    {
        Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        Error = Error.Validation(errors);
    }

    public static ValidationError Create(Dictionary<string, string[]> errors) =>
        new(errors);

    public static ValidationError Single(string field, string message) =>
        new(new Dictionary<string, string[]>
        {
            { field, new[] { message } }
        });

    public ValidationError Add(string field, string message)
    {
        var newErrors = new Dictionary<string, string[]>(Errors);

        if (newErrors.ContainsKey(field))
        {
            var existing = newErrors[field].ToList();
            existing.Add(message);
            newErrors[field] = existing.ToArray();
        }
        else
        {
            newErrors[field] = new[] { message };
        }

        return new ValidationError(newErrors);
    }

    public Result ToResult() => Result.Failure(Error);
    public Result<T> ToResult<T>() => Result<T>.Failure(Error);
}
