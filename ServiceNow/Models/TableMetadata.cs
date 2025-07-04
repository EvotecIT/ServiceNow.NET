using System.Collections.Generic;

namespace ServiceNow.Models;

/// <summary>
/// Metadata for a table.
/// </summary>
public class TableMetadata {
    public TableMetadata(string table, List<TableField> fields) {
        Table = table;
        Fields = fields;
    }

    public string Table { get; }
    public List<TableField> Fields { get; }
}
