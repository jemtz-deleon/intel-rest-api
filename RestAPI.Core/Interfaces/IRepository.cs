namespace RestAPI.Core.Interfaces;

public interface IRepository<T>
{
    Task<int> AddAsync(T entity);
    Task<int> AddRangeAsync(IEnumerable<T> entityList);
    Task<T> GetByIdAsync(object id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<int> UpdateAsync(T entity);
    Task<int> DeleteAsync(object id);
}