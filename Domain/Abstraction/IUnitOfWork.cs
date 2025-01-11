namespace Domain.Abstraction;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default); // Tranzaksiya boshlash
    Task CommitTransactionAsync(CancellationToken cancellationToken = default); // Tranzaksiya yakunlash
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default); // Tranzaksiyani bekor qilish
}
