

using MediatR;

namespace NewsAggregator.Data.CQS.Commands;

public class UpdateArticleTextByIdCommand : IRequest
{
    public Guid Id { get; set; }
    public string Text { get; set; }
}