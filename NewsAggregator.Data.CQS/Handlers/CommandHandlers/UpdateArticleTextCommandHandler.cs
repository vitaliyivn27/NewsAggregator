using MediatR;
using Microsoft.EntityFrameworkCore;
using NewsAggregator.Data.CQS.Commands;
using NewsAggregator.DataBase;

namespace NewsAggregator.Data.CQS.Handlers.CommandHandlers;

public class UpdateArticleTextCommandHandler
    : IRequestHandler<UpdateArticleTextByIdCommand, Unit>
{
    private readonly GoodNewsAggregatorContext _context;

    public UpdateArticleTextCommandHandler(GoodNewsAggregatorContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateArticleTextByIdCommand byIdCommand, CancellationToken token)
    {
        var text = byIdCommand.Text;
        var article = await _context.Articles
            .FirstOrDefaultAsync(a => a.Id.Equals(byIdCommand.Id), token);
        if (article!=null)
        {
            article.Text = text;
            await _context.SaveChangesAsync(token);
            return Unit.Value;
        }
        else
        {
            throw new ArgumentException("article doesn't exist");
        }      
    }
}