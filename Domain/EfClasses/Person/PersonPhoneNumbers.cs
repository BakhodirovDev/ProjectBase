using Domain.Abstraction.Base;

namespace Domain.EfClasses;

public class PersonPhoneNumbers : AuditableEntity
{
    private PersonPhoneNumbers() : base()
    {
    }

    public Guid PersonId { get; set; }
    public string PhoneNumber { get; set; } = null!;
}
