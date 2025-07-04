using ServiceNow.Clients;
using System.Net;
using System.Net.Http;

namespace ServiceNow.Tests;

public class TableMetadataClientTests {
    private static (TableMetadataClient Client, MockServiceNowClient Mock) Create() {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent("{\"result\":[{\"element\":\"sys_id\",\"internal_type\":\"string\"}]}")
            }
        };
        return (new TableMetadataClient(mock), mock);
    }

    [Fact]
    public async Task GetMetadataAsync_ReturnsFields() {
        var (client, mock) = Create();

        var meta = await client.GetMetadataAsync("task", CancellationToken.None);

        Assert.Equal(HttpMethod.Get, mock.LastMethod);
        Assert.Equal("/api/now/table/sys_dictionary?sysparm_query=name=task&sysparm_fields=element,internal_type", mock.LastRelativeUrl);
        Assert.Single(meta.Fields);
        Assert.Equal("sys_id", meta.Fields[0].Name);
        Assert.Equal("string", meta.Fields[0].Type);
    }

    [Fact]
    public async Task GetMetadataAsync_Error_Throws() {
        var mock = new MockServiceNowClient {
            Response = new HttpResponseMessage(HttpStatusCode.BadRequest) {
                Content = new StringContent("bad")
            }
        };
        var client = new TableMetadataClient(mock);

        var ex = await Assert.ThrowsAsync<ServiceNowException>(() => client.GetMetadataAsync("task", CancellationToken.None));
        Assert.Equal(HttpStatusCode.BadRequest, ex.StatusCode);
        Assert.Equal("bad", ex.Content);
    }
}
