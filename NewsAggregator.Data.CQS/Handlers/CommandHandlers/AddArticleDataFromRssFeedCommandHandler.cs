using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NewsAggregator.Data.CQS.Commands;
using NewsAggregator.DataBase;
using NewsAggregator.DataBase.Entities;

namespace NewsAggregator.Data.CQS.Handlers.CommandHandlers;

public class AddArticleDataFromRssFeedCommandHandler 
    : IRequestHandler<AddArticleDataFromRssFeedCommand, Unit>
{
    private readonly GoodNewsAggregatorContext _context;
    private readonly IMapper _mapper;

    public AddArticleDataFromRssFeedCommandHandler(GoodNewsAggregatorContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(AddArticleDataFromRssFeedCommand command, CancellationToken token)
    {
        var oldArticleUrls = await _context.Articles
            .Select(article => article.SourceUrl)
            .Distinct()
            .ToArrayAsync(token);
       
        var ent = command.Articles
            .Where(dto => !oldArticleUrls.Contains(dto.SourceUrl))
            .Select(dto => _mapper.Map<Article>(dto)).ToArray();

        await _context.Articles.AddRangeAsync(ent);
        await _context.SaveChangesAsync(token);
        return Unit.Value;
    }
}