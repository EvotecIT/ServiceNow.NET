namespace ServiceNow.Models;

/// <summary>
/// Represents a knowledge category.
/// </summary>
public class KnowledgeCategory {
    /// <summary>
    /// Gets or sets the unique identifier of the category.
    /// </summary>
    public string? SysId { get; set; }

    /// <summary>
    /// Gets or sets the category name.
    /// </summary>
    public string? Name { get; set; }
}
