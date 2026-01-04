using Domain.Abstraction.Base;

namespace Domain.EfClasses.Info;

public class InfoMfy : AuditableEntity<int>
{
    private InfoMfy(int id) : base(id)
    {
    }

    public string Code { get; set; } = null!;
    public string ShortName { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public int RegionId { get; set; }
    public int DistrictId { get; set; }

    // Navigation properties
    public virtual InfoRegion Region { get; private set; } = null!;
    public virtual InfoDistrict District { get; private set; } = null!;
}