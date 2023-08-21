using System.Linq.Expressions;
using CartEase.Core.Entity;

namespace CartEase.Core.Repository;

public interface IRepository
{
    public Task<bool> AnyAsync<TEntity>(int id) where TEntity : Entity.Entity;

    public IQueryable<TEntity> GetAll<TEntity>() where TEntity : Entity.Entity;

    public Task<TEntity> GetByIdAsync<TEntity>(int id) where TEntity : Entity.Entity;

    Task<TEntity> AddAsync<TEntity>(TEntity entity,
        CancellationToken cancellationToken = new CancellationToken()) where TEntity : Entity.Entity;

    Task<bool> DeleteAsync<TEntity>(TEntity entity,
        CancellationToken cancellationToken = new CancellationToken()) where TEntity : Entity.Entity;

    public Task<TEntity> UpdateAsync<TEntity>(TEntity entity,
        CancellationToken cancellationToken = new CancellationToken()) where TEntity : Entity.Entity;
}