using Domain.Abstraction.Base;
using Domain.EfClasses.Info;
using Domain.Models.Enums;

namespace Domain.EfClasses;

public class Person : AuditableEntity
{
    private Person() : base()
    {
    }

    public string FirstName { get; set; } = null!;
    public string MiddleName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string ShortName { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public int GenderId { get; set; }
    public Guid? PhotoId { get; set; }
    public int NationalityId { get; set; }
    public int CountryId { get; set; }
    public int RegionId { get; set; }
    public int DistrictId { get; set; }
    public int? MfyId { get; set; }
    public string? Address { get; set; }

    // Navigation Propertys
    public virtual InfoNationality Nationality { get; set; } = null!;
    public virtual InfoCountry Country { get; set; } = null!;
    public virtual InfoRegion Region { get; set; } = null!;
    public virtual InfoDistrict District { get; set; } = null!;
    public virtual InfoMfy? Mfy { get; set; }
    public virtual EnumGender Gender { get; set; } = null!;
    public virtual ICollection<PersonPhoneNumbers> PhoneNumbers { get; set; } = new List<PersonPhoneNumbers>();
}