using Domain.Abstraction.Base;

namespace Domain.EfClasses.Info;

public class InfoNationality : AuditableEntity<int>
{
    private InfoNationality(int id) : base(id)
    {
    }

    public string Code { get; set; } = null!;
    public string ShortName { get; set; } = null!;
    public string FullName { get; set; } = null!;
}