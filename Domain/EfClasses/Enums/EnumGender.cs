using Domain.Abstraction.Base;

namespace Domain.Models.Enums;

public class EnumGender : AuditableEntity<int>
{
    private EnumGender(int id) : base(id)
    {
    }

    public string Code { get; set; } = null!;
    public string ShortName { get; set; } = null!;
    public string FullName { get; set; } = null!;    
}