using ServiceNow.Clients;
using ServiceNow.Configuration;
using ServiceNow.Enums;
using ServiceNow.Extensions;
using System.Net;
using System.Net.Http;

namespace ServiceNow.Tests;

public class TaskStateExtensionsTests {
    private static (TableApiClient Client, MockServiceNowClient Mock) Create() {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") }
        };
        var settings = new ServiceNowSettings();
        return (new TableApiClient(mock, settings), mock);
    }

    [Fact]
    public async Task SetStateAsync_UpdatesRecord() {
        var (client, mock) = Create();

        await client.SetStateAsync("incident", "1", IncidentState.Resolved, CancellationToken.None);

        Assert.Equal(HttpMethod.Put, mock.LastMethod);
        Assert.Equal("/api/now/v2/table/incident/1", mock.LastRelativeUrl);
        Assert.NotNull(mock.LastPayload);
    }

    [Fact]
    public async Task SetStateAsync_InvalidEnum_Throws() {
        var (client, _) = Create();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => client.SetStateAsync("incident", "1", (IncidentState)99, CancellationToken.None));
    }
}
