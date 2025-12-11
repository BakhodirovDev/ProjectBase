using Domain.Abstraction.Base;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    private readonly ILoggerFactory _loggerFactory;
    private readonly Dictionary<Type, object> _repositories;

    public UnitOfWork(ApplicationDbContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _repositories = new Dictionary<Type, object>();
        _loggerFactory = loggerFactory;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }

    public IBaseRepository<TEntity> Repository<TEntity>() where TEntity : Entity
    {
        var type = typeof(TEntity);

        if (!_repositories.ContainsKey(type))
        {
            var logger = _loggerFactory.CreateLogger<Repository<TEntity>>();
            var repository = new Repository<TEntity>(_context, logger);
            _repositories[type] = repository;
        }

        return (IBaseRepository<TEntity>)_repositories[type];
    }
}