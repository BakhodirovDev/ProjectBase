namespace Domain.Abstraction.Extensions;

public static class StringExtensions
{
    public static bool NullOrWhiteSpace(this string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static bool NullOrEmpty(this string value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static TEnum AsEnum<TEnum>(this string value) where TEnum : struct
    {
        if (Enum.TryParse<TEnum>(value, out var result))
        {
            return result;
        }

        return default(TEnum);
    }

    public static object? AsEnum(this string value, Type enumType)
    {
        if (Enum.TryParse(enumType, value, out object? result))
        {
            return result;
        }

        return null;
    }
}