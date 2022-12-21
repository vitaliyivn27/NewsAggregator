

using MediatR;
using NewsAggregator.Data.CQS.Queries;
using NewsAggregator.DataBase;

namespace NewsAggregator.Data.CQS.Handlers.QueryHandlers;

public class GetArticlesNumberQueryHandler : IRequestHandler<GetArticlesNumberQuery, int>
{
    private readonly GoodNewsAggregatorContext _context;

    public GetArticlesNumberQueryHandler(GoodNewsAggregatorContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(GetArticlesNumberQuery request, CancellationToken cancellationToken)
    {
        var articlesQueryable = _context.Articles.AsQueryable();
        if (request.SourceId.HasValue)
        {
            articlesQueryable = articlesQueryable
                .Where(article => article.SourceId.Equals(request.SourceId.Value));
        }

        return articlesQueryable.Count();
    }
}