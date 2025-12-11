namespace Domain.Abstraction.Extensions;

public static class ConvertExtensions
{
    public static string? AsString(this object value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        if (value is string)
        {
            return (string)value;
        }

        return value?.ToString();
    }
}