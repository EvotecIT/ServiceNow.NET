using ServiceNow.Extensions;
using System.Collections.Generic;

namespace ServiceNow.Tests;

public class QueryStringExtensionsTests {
    [Fact]
    public void ToQueryString_ListValue_AppendsMultiplePairs() {
        var dict = new Dictionary<string, object?> { ["ids"] = new[] { "1", "2" } };

        var qs = dict.ToQueryString();

        Assert.Equal("ids=1&ids=2", qs);
    }

    [Fact]
    public void ToQueryString_MixedValues_EncodedCorrectly() {
        var dict = new Dictionary<string, object?> {
            ["fields"] = new[] { "sys_id", "number" },
            ["query"] = "active=true"
        };

        var qs = dict.ToQueryString();

        Assert.Equal("fields=sys_id&fields=number&query=active%3Dtrue", qs);
    }
}
