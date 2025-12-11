namespace Domain.Abstraction.Base;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedAt { get; }
    Guid? DeletedBy { get; }
}
