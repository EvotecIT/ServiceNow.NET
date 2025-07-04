using System.Reflection;
using System.Collections.Concurrent;
using System.Linq;

namespace ServiceNow.Utilities;

public static class ObjectExtensions {
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache = new();

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