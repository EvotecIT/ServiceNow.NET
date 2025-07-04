using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServiceNow.Utilities;

public static class ServiceNowJson {
    public static readonly JsonSerializerOptions Default = CreateDefault();

    private static JsonSerializerOptions CreateDefault() {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new EnumMemberJsonConverterFactory());
        return options;
    }
}
