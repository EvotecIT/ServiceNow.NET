namespace ServiceNow.Models;

/// <summary>
/// Represents a knowledge article.
/// </summary>
public class KnowledgeArticle {
    /// <summary>
    /// Gets or sets the unique identifier of the article.
    /// </summary>
    public string? SysId { get; set; }

    /// <summary>
    /// Gets or sets the article number.
    /// </summary>
    public string? Number { get; set; }

    /// <summary>
    /// Gets or sets the article short description.
    /// </summary>
    public string? ShortDescription { get; set; }
}
