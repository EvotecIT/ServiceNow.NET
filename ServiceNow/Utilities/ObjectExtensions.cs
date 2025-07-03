using System.Reflection;

namespace ServiceNow.Utilities;

public static class ObjectExtensions {
    public static IDictionary<string, string?> ToDictionary(this object value) {
        var dict = new Dictionary<string, string?>();
        foreach (var prop in value.GetType().GetTypeInfo().DeclaredProperties) {
            var propValue = prop.GetValue(value)?.ToString();
            dict[prop.Name] = propValue;
        }

        return dict;
    }
}