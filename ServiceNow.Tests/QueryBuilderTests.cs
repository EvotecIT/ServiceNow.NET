using ServiceNow.Queries;
using ServiceNow.Extensions;

namespace ServiceNow.Tests;

public class QueryBuilderTests {
    [Fact]
    public void BuildQuery_WithOperators_ConstructsCorrectString() {
        var qb = new QueryBuilder()
            .And("active=true")
            .Or("priority=1")
            .NewQuery()
            .And("assigned_to=me");

        var query = qb.ToQueryString();

        Assert.Equal("active=true^ORpriority=1^NQassigned_to=me", query);
    }

    [Fact]
    public void BuildQuery_SingleAnd_ReturnsCondition() {
        var qb = new QueryBuilder().And("state=1");

        Assert.Equal("state=1", qb.ToQueryString());
    }
}
