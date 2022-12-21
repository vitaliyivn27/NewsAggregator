

using Microsoft.EntityFrameworkCore;
using NewsAggregator.Data.Abstractions.Repositories;
using NewsAggregator.DataBase;
using NewsAggregator.DataBase.Entities;
using Serilog;

namespace NewsAggregator.Data.Repositories;

public class ArticleGenericRepository : Repository<Article>, IAdditionalArticleRepository
{
    public ArticleGenericRepository(GoodNewsAggregatorContext database) 
        : base(database)
    {
    }

    public async Task UpdateArticleTextAsync(Guid id, string text)
    {
        try
        {
            var article = await DbSet.FirstOrDefaultAsync(a => a.Id.Equals(id));
            if (article != null)
            {
                article.Text = text;
            }
        }
        
        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : UpdateArticleTextAsync");
            throw;
        }
    }

    //public async Task UpdateArticleRateAsync(Guid id, double rate)
    //{
    //    try
    //    {
    //        var article = await DbSet.FirstOrDefaultAsync(a => a.Id.Equals(id));
    //        if (article != null)
    //        {
    //            article.Rate = rate;
    //        }
    //    }
        
    //    catch (Exception exception)
    //    {
    //        Log.Error(exception, "MethodName : UpdateArticleRateAsync");
    //        throw;
    //    }
    //}
}