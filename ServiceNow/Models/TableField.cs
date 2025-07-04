namespace ServiceNow.Models;

/// <summary>
/// Metadata for a single field.
/// </summary>
public class TableField {
    public TableField(string name, string type) {
        Name = name;
        Type = type;
    }

    public string Name { get; }
    public string Type { get; }
}
