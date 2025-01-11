namespace Domain.Abstraction;

public class BaseParams : Entity
{
    protected BaseParams(Guid id) : base(id)
    {
        CreateAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    protected BaseParams(DateTime? updateAt)
    {
        updateAt = UpdateAt;
    }

    protected BaseParams(bool isDelete)
    {
        IsDeleted = isDelete;
        if (isDelete)
        {
            DeleteAt = DateTime.UtcNow;
        }
    }
    protected BaseParams()
    {
    }

    public DateTime CreateAt { get; private set; }
    public DateTime? UpdateAt { get; private set; }
    public DateTime? DeleteAt { get; private set; }
    public bool IsDeleted { get; private set; } = false;
}
