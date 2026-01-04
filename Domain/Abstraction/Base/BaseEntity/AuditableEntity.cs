using Consts;
using Domain.Models.Enums;

namespace Domain.Abstraction.Base;

public abstract class AuditableEntity<TId> : Entity<TId>, IAuditableEntity
    where TId : notnull
{
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
    public Guid? CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public int StatusId { get; private set; }

    public virtual EnumStatus Status { get; private set; } = null!;

    protected AuditableEntity(TId id) : base(id)
    {
        CreatedAt = DateTime.UtcNow;
        StatusId = StatusIdConst.CREATED;
    }

    protected AuditableEntity(TId id, Guid? createdBy) : base(id)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        StatusId = StatusIdConst.CREATED;
    }

    protected AuditableEntity(TId id, int statusId) : base(id)
    {
        CreatedAt = DateTime.UtcNow;
        StatusId = statusId;
    }

    /// <summary>
    /// Marks entity as created by User
    /// </summary>
    public void SetCreatedBy(Guid userId)
    {
        CreatedBy = userId;
    }

    /// <summary>
    /// Sets the entity ID (used for deserialization)
    /// </summary>
    public void IdSet(TId id)
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
    /// Changes the entity status
    /// </summary>
    public void ChangeStatus(int statusId, Guid? userId = null)
    {
        if (StatusId == statusId)
            return;

        StatusId = statusId;
        MarkAsUpdated(userId);
    }

    /// <summary>
    /// Activates the entity
    /// </summary>
    public void Activate(Guid? userId = null)
    {
        ChangeStatus(StatusIdConst.CREATED, userId);
    }


    /// <summary>
    /// Soft deletes the entity (changes status to DELETED)
    /// </summary>
    public void MarkAsDeleted(Guid? userId = null)
    {
        if (IsDeleted())
            return;

        ChangeStatus(StatusIdConst.DELETED, userId);
    }

    /// <summary>
    /// Restores a soft deleted entity (changes status to CREATED)
    /// </summary>
    public void MarkAsRestored(Guid? userId = null)
    {
        if (!IsDeleted())
            return;

        ChangeStatus(StatusIdConst.CREATED, userId);
    }

    /// <summary>
    /// Checks if entity is deleted
    /// </summary>
    public bool IsDeleted() => StatusId == StatusIdConst.DELETED;

    /// <summary>
    /// Checks if entity is active
    /// </summary>
    public bool IsActive() => StatusId == StatusIdConst.CREATED;

    /// <summary>
    /// Checks if entity can be deleted
    /// </summary>
    public virtual bool CanDelete()
    {
        return !IsDeleted();
    }

    /// <summary>
    /// Checks if entity can be restored
    /// </summary>
    public virtual bool CanRestore()
    {
        return IsDeleted();
    }

    /// <summary>
    /// Used by EF Core for materialization
    /// </summary>
    protected void SetAuditFields(
        DateTime createdAt,
        Guid? createdBy = null,
        DateTime? updatedAt = null,
        Guid? updatedBy = null,
        int statusId = StatusIdConst.CREATED)
    {
        CreatedAt = createdAt;
        CreatedBy = createdBy;
        UpdatedAt = updatedAt;
        UpdatedBy = updatedBy;
        StatusId = statusId;
    }
}

public abstract class AuditableEntity : AuditableEntity<Guid>
{
    protected AuditableEntity() : base(Guid.NewGuid()) { }
    protected AuditableEntity(Guid id) : base(id) { }
    protected AuditableEntity(Guid? createdBy) : base(Guid.NewGuid(), createdBy) { }
}