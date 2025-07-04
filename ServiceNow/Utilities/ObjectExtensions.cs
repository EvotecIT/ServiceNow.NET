using System.Reflection;
using System.Collections.Concurrent;
using System.Linq;

namespace ServiceNow.Utilities;

/// <summary>
/// Extension methods for working with generic objects.
/// </summary>
public static class ObjectExtensions {
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache = new();

    /// <summary>
    /// Converts an object's properties to a dictionary.
    /// </summary>
    /// <param name="value">The source object.</param>
    public static IDictionary<string, string?> ToDictionary(this object value) {
        var dict = new Dictionary<string, string?>();
        var properties = _propertyCache.GetOrAdd(value.GetType(), t => t.GetTypeInfo().DeclaredProperties.ToArray());
        foreach (var prop in properties) {
            var propValue = prop.GetValue(value)?.ToString();
            dict[prop.Name] = propValue;
        }

        return dict;
    }
}