namespace Domain.Abstraction.Base;

public abstract class AuditableEntity : Entity, IAuditableEntity, ISoftDeletable
{
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }
    public bool IsDeleted { get; private set; }

    protected AuditableEntity() : base()
    {
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    protected AuditableEntity(Guid id) : base(id)
    {
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    protected AuditableEntity(Guid? createdBy) : base()
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    /// <summary>
    /// Marks entity as created by user
    /// </summary>
    public void SetCreatedBy(Guid userId)
    {
        CreatedBy = userId;
    }

    /// <summary>
    /// Sets the entity ID (used for deserialization)
    /// </summary>
    public void IdSet(Guid id)
    {
        SetId(id);
    }

    /// <summary>
    /// Updates the entity and records who made the change
    /// </summary>
    public void MarkAsUpdated(Guid? userId = null)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = userId;
    }

    /// <summary>
    /// Soft deletes the entity
    /// </summary>
    public void MarkAsDeleted(Guid? userId = null)
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = userId;
    }

    /// <summary>
    /// Restores a soft deleted entity
    /// </summary>
    public void MarkAsRestored(Guid? userId = null)
    {
        if (!IsDeleted)
            return;

        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = userId;
    }

    /// <summary>
    /// Checks if entity can be deleted
    /// </summary>
    public virtual bool CanDelete()
    {
        return !IsDeleted;
    }

    /// <summary>
    /// Checks if entity can be restored
    /// </summary>
    public virtual bool CanRestore()
    {
        return IsDeleted;
    }

    /// <summary>
    /// Used by EF Core for materialization
    /// </summary>
    protected void SetAuditFields(
        DateTime createdAt,
        Guid? createdBy = null,
        DateTime? updatedAt = null,
        Guid? updatedBy = null,
        DateTime? deletedAt = null,
        Guid? deletedBy = null,
        bool isDeleted = false)
    {
        CreatedAt = createdAt;
        CreatedBy = createdBy;
        UpdatedAt = updatedAt;
        UpdatedBy = updatedBy;
        DeletedAt = deletedAt;
        DeletedBy = deletedBy;
        IsDeleted = isDeleted;
    }
}
