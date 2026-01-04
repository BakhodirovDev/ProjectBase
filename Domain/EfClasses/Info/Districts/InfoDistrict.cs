using Domain.Abstraction.Base;

namespace Domain.EfClasses.Info;

public class InfoDistrict : AuditableEntity<int>
{
    private InfoDistrict(int id) : base(id)
    {
    }

    public string Code { get; set; } = null!;
    public string? Soato { get; set; }
    public string? RoamingCode { get; set; }
    public string ShortName { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public bool IsCenter { get; set; }
    public int RegionId { get; set; }

    // Navigation properties
    public virtual InfoRegion Region { get; private set; } = null!;
    public virtual ICollection<InfoMfy> Mfys { get; private set; } = new List<InfoMfy>();
}