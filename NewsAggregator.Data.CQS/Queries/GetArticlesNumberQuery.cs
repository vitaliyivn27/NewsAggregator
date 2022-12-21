using MediatR;

namespace NewsAggregator.Data.CQS.Queries;

public class GetArticlesNumberQuery : IRequest<int>
{
    public Guid? SourceId { get; set; }
}