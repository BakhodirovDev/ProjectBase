namespace Base;

public interface IEntity
{
    Guid Id { get; }
}

public interface IEntity<TId> where TId : notnull
{
    TId Id { get; }
}