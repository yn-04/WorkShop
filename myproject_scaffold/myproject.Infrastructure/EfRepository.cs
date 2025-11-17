using Microsoft.EntityFrameworkCore;
using myproject.Core;
using myproject.Core.Interfaces;
using System.Linq.Expressions;

namespace myproject.Infrastructure;

public class EfRepository<T>(AppDbContext db) : IRepository<T> where T : class
{
    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await db.Set<T>().AddAsync(entity, ct);
    }

    public Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        db.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<T?> GetByIdAsync(object id, CancellationToken ct = default)
        => await db.Set<T>().FindAsync([id], ct);

    public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
    {
        IQueryable<T> q = db.Set<T>();
        if (predicate is not null) q = q.Where(predicate);
        return await q.AsNoTracking().ToListAsync(ct);
    }

    public Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        db.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
