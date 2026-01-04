using Domain.Abstraction.Base;

namespace Domain.EfClasses.Authentication;

public class RolePermission : AuditableEntity
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }

    // navigation properties
    public virtual Role Role { get; set; } = null!;
    public virtual Permission Permission { get; set; } = null!;
}