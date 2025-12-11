namespace Domain.Abstraction.Base;

/// <summary>
/// Base entity interface
/// </summary>
public interface IEntity
{
    Guid Id { get; set; }
}

/// <summary>
/// Soft delete support
/// </summary>
public interface ISoftDelete
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
    Guid? DeletedBy { get; set; }
}

/// <summary>
/// Active/Inactive support
/// </summary>
public interface IActivatable
{
    bool IsActive { get; set; }
}

/// <summary>
/// Audit support
/// </summary>
public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    Guid? CreatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    Guid? UpdatedBy { get; set; }
}

/// <summary>
/// Full entity with all features
/// </summary>
public interface IFullEntity : IEntity, ISoftDelete, IActivatable, IAuditable
{
}
