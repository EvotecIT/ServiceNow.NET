using System.Reflection;

namespace ServiceNow.Utilities;

/// <summary>
/// Helper extension methods for object instances.
/// </summary>
public static class ObjectExtensions {
    /// <summary>
    /// Converts an object's public properties to a dictionary of strings.
    /// </summary>
    /// <param name="value">The object to convert.</param>
    /// <returns>A dictionary containing property names and values.</returns>
    public static IDictionary<string, string?> ToDictionary(this object value) {
        var dict = new Dictionary<string, string?>();
        foreach (var prop in value.GetType().GetTypeInfo().DeclaredProperties) {
            var propValue = prop.GetValue(value)?.ToString();
            dict[prop.Name] = propValue;
        }

        return dict;
    }
}