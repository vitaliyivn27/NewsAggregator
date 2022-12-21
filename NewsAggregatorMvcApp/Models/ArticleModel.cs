

using System.ComponentModel.DataAnnotations;

namespace NewsAggregatorMvcApp.Models;

public class ArticleModel
{
    public Guid Id { get; set; }
    
    [Required]
    [MinLength(3)]
    [MaxLength(256)]
    public string? Title { get; set; }

    [Required]
    public string? Category { get; set; }

    [Required]
    public string? ShortSummary { get; set; }

    [Required]
    public string? Text { get; set; }

    public DateTime PublicationDate { get; set; }
}