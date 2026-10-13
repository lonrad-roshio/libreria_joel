using LibreriaJoel.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibreriaJoel.Repositories;

public class GenericRepository<T> : IRepository<T> where T : class
{
    private readonly DbContext _database;
    protected readonly DbSet<T> Data;

    public GenericRepository(DbContext database)
    {
        _database = database;
        Data = database.Set<T>();
    }

    public async Task<int> InsertAsync(T entity)
    {
        await Data.AddAsync(entity);
        return await _database.SaveChangesAsync();
    }

    public Task<List<T>> SelectAsync()
    {
        var readOnlyData = Data.AsNoTracking();
        return readOnlyData.ToListAsync();
    }

    public virtual async Task<T?> FindAsync(int id)
    {
        return await Data.FindAsync(id);
    }

    public async Task<int> UpdateAsync(T entity)
    {
        Data.Update(entity);
        return await _database.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(T entity)
    {
        Data.Remove(entity);
        return await _database.SaveChangesAsync();
    }
}
