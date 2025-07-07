namespace ServiceNow.Models;

/// <summary>
/// Represents a ServiceNow problem record.
/// </summary>
public class Problem {
    /// <summary>
    /// Gets or sets the unique identifier of the problem record.
    /// </summary>
    public string? SysId { get; set; }

    /// <summary>
    /// Gets or sets the problem number.
    /// </summary>
    public string? Number { get; set; }

    /// <summary>
    /// Gets or sets a short description of the problem.
    /// </summary>
    public string? ShortDescription { get; set; }
}
