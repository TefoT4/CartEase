using CartEase.Core.Entity;
using CartEase.Core.Repository;
using CartEase.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CartEase.Application.Repository;

public class EfRepository : IRepository, IDisposable
{
    private readonly CartEaseContext _context;

    public EfRepository(CartEaseContext context)
    {
        _context = context;
    }

    public Task<bool> AnyAsync<TEntity>(int id) where TEntity : Entity
    {
        return _context.Set<TEntity>().AnyAsync(e => e.Id == id);
    }

    public IQueryable<TEntity> GetAll<TEntity>() where TEntity : Entity
    {
        return _context.Set<TEntity>().AsQueryable();
    }

    public Task<TEntity?> GetByIdAsync<TEntity>(int id) where TEntity : Entity
    {
        return _context.Set<TEntity>().FirstOrDefaultAsync(e => e.Id == id);
    }

    public Task<TEntity> AddAsync<TEntity>(TEntity entity, int currentUserId, 
        CancellationToken cancellationToken = new CancellationToken()) where TEntity : Entity
    {
        _context.Set<TEntity>().Add(entity);
        _context.SaveChangesAsync(cancellationToken);
        return Task.FromResult(entity);
    }

    public Task<bool> DeleteAsync<TEntity>(TEntity entity, int currentUserId, 
        CancellationToken cancellationToken = new CancellationToken()) where TEntity : Entity
    {
        _context.Set<TEntity>().Remove(entity);
        _context.SaveChangesAsync(cancellationToken);
        return Task.FromResult(true);
    }

    public Task<TEntity> UpdateAsync<TEntity>(TEntity entity, int currentUserId, 
        CancellationToken cancellationToken = new CancellationToken()) where TEntity : Entity
    {
        _context.Set<TEntity>().Update(entity);
        _context.SaveChangesAsync(cancellationToken);
        return Task.FromResult(entity);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}