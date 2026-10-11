using System;
using Microsoft.EntityFrameworkCore.Storage;
using Remp.DataAccess.Data;

namespace Remp.Repository.Common;

public class UnitOfWork : IUnitOfWork
{
    private readonly RempDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(RempDbContext context)
    {
        _context = context;
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No active transaction to commit.");
        }

        await _transaction.CommitAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackAsync()
    {
        if (_transaction == null)
        {
            return;
        }

        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

}
