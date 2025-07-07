using System.Collections.Generic;

namespace ServiceNow.Models;

/// <summary>
/// Metadata for a table.
/// </summary>
public class TableMetadata {
    /// <summary>
    /// Initializes a new instance of the <see cref="TableMetadata"/> class.
    /// </summary>
    /// <param name="table">Table name.</param>
    /// <param name="fields">List of fields.</param>
    public TableMetadata(string table, List<TableField> fields) {
        Table = table;
        Fields = fields;
    }

    /// <summary>
    /// Gets the table name.
    /// </summary>
    public string Table { get; }

    /// <summary>
    /// Gets the collection of fields.
    /// </summary>
    public List<TableField> Fields { get; }
}
