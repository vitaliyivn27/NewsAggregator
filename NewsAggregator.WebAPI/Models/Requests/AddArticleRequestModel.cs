using System.ComponentModel.DataAnnotations;

namespace NewsAggregator.WebAPI.Models.Requests;

/// <summary>
/// 
/// </summary>
public class AddArticleRequestModel
{
    /// <summary>
    /// 
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Required]
    public string Title { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? ShortSummary { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double? Rate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime? PublicationDate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Required]
    public string SourceUrl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Required]
    public Guid SourceId { get; set; }
}