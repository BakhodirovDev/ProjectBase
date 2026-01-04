using Domain.Abstraction.Base;

namespace Domain.EfClasses.Authentication;

public class Role : AuditableEntity
{
    private Role() : base()
    {
        
    }
    
    private bool _isSystemRole = false;
    private bool _isSystemAdmin = false;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsDefault { get; set; } = false;

    public bool IsSystemRole
    {
        get => _isSystemRole;
        set
        {
            if (value)
            {
                ResetAllRoles();
                _isSystemRole = true;
            }
            else
            {
                _isSystemRole = false;
            }
        }
    }

    public bool IsSystemAdmin
    {
        get => _isSystemAdmin;
        set
        {
            if (value)
            {
                ResetAllRoles();
                _isSystemAdmin = true;
                _isSystemRole = true;
            }
            else
            {
                _isSystemAdmin = false;
            }
        }
    }

    // navigation properties
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    private void ResetAllRoles()
    {
        _isSystemRole = false;
        _isSystemAdmin = false;
    }
}