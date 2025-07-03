using ServiceNow.Builders;

namespace ServiceNow.Tests;

public class UnitTest1
{
    [Fact]
    public void QueryBuilderBuildsExpectedString()
    {
        var query = new QueryBuilder()
            .Add("number", "INC001")
            .AddIfNotNull("state", "New")
            .AddIfNotNull("empty", null)
            .Build();

        var parts = query.Split('&').OrderBy(p => p).ToArray();
        Assert.Equal(new[] { "number=INC001", "state=New" }, parts);
    }
}
