

using Microsoft.EntityFrameworkCore;
using NewsAggregator.Core;
using NewsAggregator.Data.Abstractions.Repositories;
using NewsAggregator.DataBase;
using NewsAggregator.DataBase.Entities;
using Serilog;
using System.Linq.Expressions;

namespace NewsAggregator.Data.Repositories;

public class Repository<T> : IRepository<T> 
    where T: class, IBaseEntity
{
    protected readonly GoodNewsAggregatorContext Database;
    protected readonly DbSet<T> DbSet;
    
    public Repository(GoodNewsAggregatorContext database)
    {
        Database = database;
        DbSet = database.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        try
        {
            return await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(entity => entity.Id.Equals(id));
        }
        
        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : GetByIdAsync");
            throw;
        }
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }

    public virtual IQueryable<T> Get()
    {
        return DbSet;
    }

    public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> searchExpression, 
        params Expression<Func<T, object>>[] includes)
    {
        try
        {
            var result = DbSet.Where(searchExpression);
            if (includes.Any())
            {
                result = includes.Aggregate(result, (current, include) =>
                    current.Include(include));
            }
            return result;
        }
        
        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : FindBy");
            throw;
        }
    }

    public virtual async Task AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await DbSet.AddRangeAsync(entities);
    }

    public virtual void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public virtual async Task PatchAsync(Guid id, List<PatchModel> patchData)
    {
        try
        {
            var model = await DbSet.FirstOrDefaultAsync(entity => entity.Id.Equals(id));

            var nameValuePropertiesPairs = patchData
                .ToDictionary(
                    patchModel => patchModel.PropertyName,
                    patchModel => patchModel.PropertyValue);

            var dbEntityEntry = Database.Entry(model);
            dbEntityEntry.CurrentValues.SetValues(nameValuePropertiesPairs);
            dbEntityEntry.State = EntityState.Modified;
        }
        
        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : PatchAsync");
            throw;
        }
    }

    public virtual void Remove(T entity)
    {
        DbSet.Remove(entity);
    }
}