

using Microsoft.EntityFrameworkCore;
using NewsAggregator.Data.Abstractions.Repositories;
using NewsAggregator.DataBase;
using NewsAggregator.DataBase.Entities;
using Serilog;

namespace NewsAggregator.Data.Repositories
{
    public class ArticleRepository : IArticleRepository //-> CRUD operations with Articles Table in DB
    {
        private readonly GoodNewsAggregatorContext _database;
        
        public ArticleRepository(GoodNewsAggregatorContext database)
        {
            _database = database;
        }

        public async Task<Article?> GetArticleByIdAsync(Guid id)
        {
            try
            {
                return await _database
                    .Articles.FirstOrDefaultAsync(article => article.Id.Equals(id));
            }
            
            catch (Exception exception)
            {
                Log.Error(exception, "MethodName : GetArticleByIdAsync");
                throw;
            }
        }
        
        //not for regular usage
        public IQueryable<Article> GetArticlesAsQueryable()
        {
            try
            {
                return _database.Articles;
            }
            
            catch (Exception exception)
            {
                Log.Error(exception, "MethodName : GetArticlesAsQueryable");
                throw;
            }
        }

        public async Task<List<Article?>> GetAllArticlesAsync()
        {
            try
            {
                return await _database.Articles.ToListAsync();
            }
            
            catch (Exception exception)
            {
                Log.Error(exception, "MethodName : GetAllArticlesAsync");
                throw;
            }
        }

        public async Task AddArticleAsync(Article article)
        {
            try
            {
                await _database.Articles.AddAsync(article);
            }
            
            catch (Exception exception)
            {
                Log.Error(exception, "MethodName : AddArticleAsync");
                throw;
            }
        }

        public async Task AddArticlesAsync(IEnumerable<Article> articles)
        {
            try
            {
                await _database.Articles.AddRangeAsync(articles);
            }
            
            catch (Exception exception)
            {
                Log.Error(exception, "MethodName : AddArticlesAsync");
                throw;
            }
        }

        public async Task RemoveArticleAsync(Article article)
        {
            try
            {
                _database.Articles.Remove(article);
            }
            
            catch (Exception exception)
            {
                Log.Error(exception, "MethodName : RemoveArticleAsync");
                throw;
            }
        }

        public async Task UpdateArticle(Guid id, Article article)
        {
            try
            {
                var entity = await _database.Articles.FirstOrDefaultAsync(article => article.Id.Equals(id));

                if (entity != null)
                {
                    entity = article;
                }
            }
            
            catch (Exception exception)
            {
                Log.Error(exception, "MethodName : UpdateArticle");
                throw;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _database.SaveChangesAsync();
        }
    }
}