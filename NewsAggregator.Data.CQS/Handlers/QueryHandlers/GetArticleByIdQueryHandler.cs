

using MediatR;
using Microsoft.EntityFrameworkCore;
using NewsAggregator.Data.CQS.Queries;
using NewsAggregator.DataBase;
using NewsAggregator.DataBase.Entities;
using Serilog;

namespace NewsAggregator.Data.CQS.Handlers.QueryHandlers;

public class GetArticleByIdQueryHandler : IRequestHandler<GetArticleByIdQuery, Article?>
{
    private readonly GoodNewsAggregatorContext _context;

    public GetArticleByIdQueryHandler(GoodNewsAggregatorContext context)
    {
        _context = context;
    }

    public async Task<Article?> Handle(GetArticleByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var article = await _context.Articles
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id.Equals(request.Id), cancellationToken);
            return article;
        }
        catch(ArgumentNullException ex)
        {
            Log.Error(ex, "article doesn't exist");
            throw;
        }       
    }
}