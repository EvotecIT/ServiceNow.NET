using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServiceNow.Utilities;

/// <summary>
/// Provides JSON serialization settings used by the library.
/// </summary>
public static class ServiceNowJson {
    /// <summary>
    /// Default serializer options.
    /// </summary>
    public static readonly JsonSerializerOptions Default = CreateDefault();

    private static JsonSerializerOptions CreateDefault() {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new EnumMemberJsonConverterFactory());
        return options;
    }
}
