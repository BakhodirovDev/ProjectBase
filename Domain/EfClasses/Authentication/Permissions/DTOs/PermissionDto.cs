namespace Domain.EfClasses.Authentication;

/// <summary>
/// Permission DTO - read operations uchun
/// </summary>
public class PermissionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Resource { get; set; } = null!;
    public string Action { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Permission yaratish uchun DTO
/// </summary>
public class CreatePermissionDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Resource { get; set; } = null!;
    public string Action { get; set; } = null!;
}

/// <summary>
/// Permission yangilash uchun DTO
/// </summary>
public class UpdatePermissionDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Resource { get; set; }
    public string? Action { get; set; }
}