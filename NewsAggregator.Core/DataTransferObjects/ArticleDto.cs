namespace NewsAggregator.Core.DataTransferObjects;

public class ArticleDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Category { get; set; }
    public string? ShortSummary { get; set; }   
    public string? Text { get; set; }
    public double? Rate { get; set; }
    public string SourceUrl { get; set; }
    public DateTime PublicationDate { get; set; }
    public Guid SourceId { get; set; }
}