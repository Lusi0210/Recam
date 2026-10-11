using System;

namespace Remp.Repository.Common;

public interface IUnitOfWork
{
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
    Task<int> SaveChangesAsync();
}
