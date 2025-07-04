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

    public string Table { get; }
    public List<TableField> Fields { get; }
}
