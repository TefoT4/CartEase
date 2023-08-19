using CartEase.Core.Entity;

namespace CartEase.Core.Repository;

public interface IRepository
{
    public Task<bool> AnyAsync<TEntity>(int id) where TEntity : Entity.Entity;

    public IQueryable<TEntity> GetAllAsync<TEntity>() where TEntity : Entity.Entity;

    public Task<TEntity?> GetByIdAsync<TEntity>(int id) where TEntity : Entity.Entity;

    Task<TEntity> AddAsync<TEntity>(TEntity entity, int currentUserId,
        CancellationToken cancellationToken = new CancellationToken()) where TEntity : Entity.Entity;

    Task<bool> DeleteAsync<TEntity>(TEntity entity, int currentUserId,
        CancellationToken cancellationToken = new CancellationToken()) where TEntity : Entity.Entity;

    public Task<TEntity> UpdateAsync<TEntity>(TEntity entity, int currentUserId,
        CancellationToken cancellationToken = new CancellationToken()) where TEntity : Entity.Entity;
}