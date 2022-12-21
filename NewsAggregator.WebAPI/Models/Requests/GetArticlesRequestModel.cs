namespace NewsAggregator.WebAPI.Models.Requests;

public class GetArticlesRequestModel
{
    public Guid? SourceId { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}