namespace LibreriaJoel.Repositories.Interfaces;

public interface IRepository<T> where T : class
{
    Task<int> InsertAsync(T entity);
    Task<List<T>> SelectAsync();
    Task<T?> FindAsync(int id);
    Task<int> UpdateAsync(T entity);
    Task<int> DeleteAsync(T entity);
}
