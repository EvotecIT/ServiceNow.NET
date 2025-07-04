namespace ServiceNow.Models;

/// <summary>
/// Metadata for a single field.
/// </summary>
public class TableField {
    /// <summary>
    /// Initializes a new instance of the <see cref="TableField"/> class.
    /// </summary>
    /// <param name="name">Field name.</param>
    /// <param name="type">Field type.</param>
    public TableField(string name, string type) {
        Name = name;
        Type = type;
    }

    public string Name { get; }
    public string Type { get; }
}
