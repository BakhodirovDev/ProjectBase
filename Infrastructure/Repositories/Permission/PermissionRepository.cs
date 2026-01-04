using Domain.EfClasses.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class PermissionRepository : Repository<Permission>, IPermissionRepository
{
    public PermissionRepository(DbContext context, ILogger<Repository<Permission, Guid>> logger) : base(context, logger)
    {
    }
}