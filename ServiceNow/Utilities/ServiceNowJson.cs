using System.Text.Json;

namespace ServiceNow.Utilities;

public static class ServiceNowJson {
    public static readonly JsonSerializerOptions Default = new() {
        PropertyNameCaseInsensitive = true
    };
}
