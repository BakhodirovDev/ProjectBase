using Domain.Abstraction.Base;

namespace Domain.EfClasses.Info;

public class InfoCountry : AuditableEntity<int>
{
    private InfoCountry(int id) : base(id)
    {
    }

    public string Code { get; set; } = null!;
    public string ShortName { get; set; } = null!;
    public string FullName { get; set; } = null!;

    // Navigation property - regions
    public virtual ICollection<InfoRegion> Regions { get; private set; } = new List<InfoRegion>();
}