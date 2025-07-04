using System.Text.Json;
using ServiceNow.Models;
using ServiceNow.Enums;
using ServiceNow.Utilities;
using Xunit;

namespace ServiceNow.Tests;

public class JsonSerializationTests {
    [Fact]
    public void Serialize_UsesEnumMemberValues() {
        var incident = new Incident { State = IncidentState.InProgress };
        var json = JsonSerializer.Serialize(incident, ServiceNowJson.Default);
        Assert.Contains("\"State\":\"In Progress\"", json);
    }

    [Fact]
    public void Deserialize_UsesEnumMemberValues() {
        const string json = "{\"State\":\"Resolved\"}";
        var incident = JsonSerializer.Deserialize<Incident>(json, ServiceNowJson.Default);
        Assert.NotNull(incident);
        Assert.Equal(IncidentState.Resolved, incident!.State);
    }
}
