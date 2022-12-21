using NewsAggregator.Core;
using NewsAggregator.DataBase.Entities;
using System.ComponentModel.DataAnnotations;

namespace NewsAggregator.WebAPI.Models.Requests;

/// <summary>
/// 
/// </summary>
public class AddSourceRequestModel
{
    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Required]
    public string Url { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? RssUrl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public SourceType SourceType { get; set; }
}