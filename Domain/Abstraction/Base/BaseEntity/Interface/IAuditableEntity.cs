namespace Domain.Abstraction.Base;

public interface IAuditableEntity
{
    DateTime CreatedAt { get; }
    Guid? CreatedBy { get; }
    DateTime? UpdatedAt { get; }
    Guid? UpdatedBy { get; }
}
