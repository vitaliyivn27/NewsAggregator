namespace NewsAggregator.DataBase.Entities
{
    public class Article : IBaseEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Category { get; set; }
        public string? ShortSummary { get; set; }
        public string? Text { get; set; }
        public double? Rate { get; set; }
        public DateTime PublicationDate { get; set; }
        public string SourceUrl { get; set; }
        public Guid SourceId { get; set; }
        public Source Source{ get; set; }
        public List<Comment> Comments { get; set; }
    }
}