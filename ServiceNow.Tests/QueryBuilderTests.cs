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

    [Fact]
    public void BuildQuery_AfterBefore_GeneratesDateConditions() {
        var qb = new QueryBuilder()
            .After("opened_at", new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero))
            .Before("opened_at", new DateTimeOffset(2024, 1, 31, 23, 59, 59, TimeSpan.Zero));

        Assert.Equal(
            "opened_at>=2024-01-01 00:00:00^opened_at<=2024-01-31 23:59:59",
            qb.ToQueryString());
    }

    [Fact]
    public void BuildQuery_Equals_GeneratesEqualityCondition() {
        var qb = new QueryBuilder().Equals("state", 1);

        Assert.Equal("state=1", qb.ToQueryString());
    }

    [Fact]
    public void BuildQuery_Contains_GeneratesLikeCondition() {
        var qb = new QueryBuilder().Contains("short_description", "error");

        Assert.Equal("short_descriptionLIKEerror", qb.ToQueryString());
    }

    [Fact]
    public void BuildQuery_Between_GeneratesBetweenCondition() {
        var qb = new QueryBuilder()
            .Between(
                "opened_at",
                new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2024, 1, 2, 23, 59, 59, TimeSpan.Zero));

        Assert.Equal(
            "opened_atBETWEEN2024-01-01 00:00:00@2024-01-02 23:59:59",
            qb.ToQueryString());
    }
}