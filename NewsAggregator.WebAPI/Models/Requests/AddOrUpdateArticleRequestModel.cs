﻿using System.ComponentModel.DataAnnotations;

namespace NewsAggregator.WebAPI.Models.Requests;

/// <summary>
/// 
/// </summary>
public class AddOrUpdateArticleRequestModel
{
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
}