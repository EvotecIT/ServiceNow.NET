namespace ServiceNow.Models;

/// <summary>
/// Represents a generic task record in ServiceNow.
/// </summary>
public class TaskRecord {
    /// <summary>
    /// Gets or sets the unique identifier of the task record.
    /// </summary>
    public string? SysId { get; set; }

    /// <summary>
    /// Gets or sets the task number.
    /// </summary>
    public string? Number { get; set; }

    /// <summary>
    /// Gets or sets the short description of the task.
    /// </summary>
    public string? ShortDescription { get; set; }
}