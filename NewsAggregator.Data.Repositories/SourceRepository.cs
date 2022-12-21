

using Microsoft.EntityFrameworkCore;
using NewsAggregator.Data.Abstractions.Repositories;
using NewsAggregator.DataBase;
using NewsAggregator.DataBase.Entities;
using Serilog;

namespace NewsAggregator.Data.Repositories
{
    public class SourceRepository : ISourceRepository 
    {
        private readonly GoodNewsAggregatorContext _database;
        
        public SourceRepository(GoodNewsAggregatorContext database)
        {
            _database = database;
        }

        public async Task<Source?> GetSourceByIdAsync(Guid id)
        {
            try
            {
                return await _database
                .Sources.FirstOrDefaultAsync(source => source.Id.Equals(id));
            }
            
            catch (Exception exception)
            {
                Log.Error(exception, "MethodName : GetSourceByIdAsync");
                throw;
            }
        }
        
        //not for regular usage
        public IQueryable<Source> GetSourcesAsQueryable()
        {
            return _database.Sources;
        }

        public async Task<List<Source?>> GetAllSourcesAsync()
        {
            return await _database.Sources.ToListAsync();
        }

        public async Task AddSourceAsync(Source source)
        {
            await _database.Sources.AddAsync(source);
        }

        public async Task AddSourcesAsync(IEnumerable<Source> articles)
        {
            await _database.Sources.AddRangeAsync(articles);
        }

        public async Task RemoveSourceAsync(Source source)
        {
            _database.Sources.Remove(source);
        }

        public async Task UpdateSource(Guid id, Source source)
        {
            try
            {
                var entity = await _database.Sources.FirstOrDefaultAsync(source => source.Id.Equals(id));

                if (entity != null)
                {
                    entity = source;
                }
            }
            
            catch (Exception exception)
            {
                Log.Error(exception, "MethodName : UpdateSource");
                throw;
            }
        }
    }
}