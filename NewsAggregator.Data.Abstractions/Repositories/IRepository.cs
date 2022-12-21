

using NewsAggregator.Core;
using NewsAggregator.DataBase.Entities;
using System.Linq.Expressions;

namespace NewsAggregator.Data.Abstractions.Repositories;

public interface IRepository<T> where T: IBaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    IQueryable<T> Get();
    IQueryable<T> FindBy(Expression<Func<T, bool>> searchExpression,
        params Expression<Func<T, object>>[] includes);
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    void Update(T entity);
    Task PatchAsync(Guid id, List<PatchModel> patchData);
    void Remove(T entity);
}