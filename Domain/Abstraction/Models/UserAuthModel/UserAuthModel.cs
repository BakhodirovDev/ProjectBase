using Domain.EfClasses;
using Domain.Models.Enums;

namespace Domain.Abstraction.Models;

public class UserAuthModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public string FullName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public bool IsEmailConfirmed { get; set; } = false;
    public EnumGender Gender { get; set; } = null!;
    public ICollection<Token> Tokens { get; set; } = new List<Token>();
}