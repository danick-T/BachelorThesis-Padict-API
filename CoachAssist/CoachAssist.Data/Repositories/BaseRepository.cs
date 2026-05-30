using CoachAssist.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoachAssist.Data.Repositories;

public abstract class BaseRepository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext Db;
    protected readonly DbSet<T> Set;

    protected BaseRepository(AppDbContext db)
    {
        Db = db;
        Set = db.Set<T>();
    }

    public virtual Task<T?> GetByIdAsync(int id) => Set.FindAsync(id).AsTask();

    public virtual async Task<IEnumerable<T>> GetAllAsync() => await Set.AsNoTracking().ToListAsync();

    public virtual async Task AddAsync(T entity) => await Set.AddAsync(entity);

    public virtual void Update(T entity) => Set.Update(entity);

    public virtual void Remove(T entity) => Set.Remove(entity);

    public virtual async Task<bool> ExistsAsync(int id)
    {
        var entity = await Set.FindAsync(id);
        return entity is not null;
    }

    public Task SaveChangesAsync() => Db.SaveChangesAsync();
}
