using ServiceNow.Extensions;
using System.Net.Http;

namespace ServiceNow.Tests;

public class FluentApiTests {
    [Fact]
    public void TableExtension_ReturnsBuilder() {
        using var http = new HttpClient();
        var settings = new ServiceNow.Configuration.ServiceNowSettings {
            BaseUrl = "https://example.com",
            Username = "user",
            Password = "pass"
        };
        var client = new ServiceNow.Clients.ServiceNowClient(http, settings);
        var builder = client.Table("incident");
        Assert.NotNull(builder);
        Assert.Equal("incident", builder.TableName);
    }
}