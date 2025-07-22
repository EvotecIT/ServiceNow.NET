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
    public void Serialize_CatalogApproval_UsesEnumMemberValues() {
        var approval = new CatalogApproval { State = CatalogApprovalState.Approved };
        var json = JsonSerializer.Serialize(approval, ServiceNowJson.Default);
        Assert.Contains("\"State\":\"Approved\"", json);
    }

    [Fact]
    public void Serialize_ChangeRequest_UsesEnumMemberValues() {
        var cr = new ChangeRequest { State = ChangeRequestState.Authorize };
        var json = JsonSerializer.Serialize(cr, ServiceNowJson.Default);
        Assert.Contains("\"State\":\"Authorize\"", json);
    }

    [Fact]
    public void Deserialize_UsesEnumMemberValues() {
        const string json = "{\"State\":\"Resolved\"}";
        var incident = JsonSerializer.Deserialize<Incident>(json, ServiceNowJson.Default);
        Assert.NotNull(incident);
        Assert.Equal(IncidentState.Resolved, incident!.State);
    }

    [Fact]
    public void Deserialize_CatalogApproval_UsesEnumMemberValues() {
        const string json = "{\"State\":\"Rejected\"}";
        var approval = JsonSerializer.Deserialize<CatalogApproval>(json, ServiceNowJson.Default);
        Assert.NotNull(approval);
        Assert.Equal(CatalogApprovalState.Rejected, approval!.State);
    }

    [Fact]
    public void Deserialize_ChangeRequest_UsesEnumMemberValues() {
        const string json = "{\"State\":\"Scheduled\"}";
        var cr = JsonSerializer.Deserialize<ChangeRequest>(json, ServiceNowJson.Default);
        Assert.NotNull(cr);
        Assert.Equal(ChangeRequestState.Scheduled, cr!.State);
    }
}
