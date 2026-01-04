using Domain.Abstraction.Base;

namespace Domain.Models.Enums;

public class EnumDocumentType : AuditableEntity<int>
{
    private EnumDocumentType(int id) : base(id)
    {
    }

    public string Code { get; set; } = null!;
    public string ShortName { get; set; } = null!;
    public string FullName { get; set; } = null!;
}