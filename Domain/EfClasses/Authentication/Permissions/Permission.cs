using Domain.Abstraction.Base;

namespace Domain.EfClasses.Authentication;

public class Permission : AuditableEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Resource { get; set; } = null!;
    public string Action { get; set; } = null!;

    // navigation properties
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}