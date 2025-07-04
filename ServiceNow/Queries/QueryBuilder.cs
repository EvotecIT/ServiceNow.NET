using System.Text;

namespace ServiceNow.Queries;

/// <summary>
/// Helps construct ServiceNow encoded query strings.
/// </summary>
public sealed class QueryBuilder {
    private readonly StringBuilder _builder = new();
    private bool _justStartedNewQuery;

    /// <summary>
    /// Adds a condition to the query using the '^' (AND) operator if needed.
    /// </summary>
    /// <param name="condition">Query condition.</param>
    public QueryBuilder And(string condition) {
        if (_builder.Length > 0 && !_justStartedNewQuery) {
            _builder.Append('^');
        }
        _builder.Append(condition);
        _justStartedNewQuery = false;
        return this;
    }

    /// <summary>
    /// Adds a condition preceded by the '^OR' operator.
    /// </summary>
    /// <param name="condition">Query condition.</param>
    public QueryBuilder Or(string condition) {
        if (_builder.Length > 0) {
            _builder.Append("^OR");
        }
        _builder.Append(condition);
        _justStartedNewQuery = false;
        return this;
    }

    /// <summary>
    /// Appends the '^NQ' operator to start a new query group.
    /// </summary>
    public QueryBuilder NewQuery() {
        if (_builder.Length > 0 && !_justStartedNewQuery) {
            _builder.Append("^NQ");
        }
        _justStartedNewQuery = true;
        return this;
    }

    /// <summary>
    /// Builds the encoded query string.
    /// </summary>
    public override string ToString() => _builder.ToString();
}
