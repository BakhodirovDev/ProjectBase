using Domain.Abstraction.Base;
using Domain.EfClasses.Authentication;

namespace Domain.EfClasses;

public class User : AuditableEntity
{
    public User() : base()
    {
    }

    private User(Guid id) : base(id)
    {
    }

    public Guid PersonId { get; set; }
    public string UserName { get; set; } = null!;
    public string? Email { get; set; }
    public bool IsEmailConfirmed { get; set; } = false;
    public string? PasswordHash { get; set; }

    public virtual Person Person { get; set; } = null!;
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}