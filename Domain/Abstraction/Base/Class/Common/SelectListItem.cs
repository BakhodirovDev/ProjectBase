namespace Domain.Abstraction.Base;

public abstract class SelectListItemBase
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

public class SelectListItem<TKey> : SelectListItemBase
{
    public TKey Id { get; set; } = default!;

    /// <summary>
    /// Gets metadata value by key with type safety
    /// </summary>
    public T? GetMetadata<T>(string key)
    {
        if (Metadata == null || !Metadata.TryGetValue(key, out var value))
            return default;

        try
        {
            return (T)value;
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Sets metadata value
    /// </summary>
    public void SetMetadata(string key, object value)
    {
        Metadata ??= new Dictionary<string, object>();
        Metadata[key] = value;
    }

    /// <summary>
    /// Checks if metadata key exists
    /// </summary>
    public bool HasMetadata(string key)
    {
        return Metadata?.ContainsKey(key) ?? false;
    }
}

public class SelectListItem : SelectListItem<Guid>
{
}