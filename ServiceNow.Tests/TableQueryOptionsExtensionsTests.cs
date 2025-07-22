using ServiceNow;
using ServiceNow.Extensions;
using System.Collections.Generic;

namespace ServiceNow.Tests;

public class TableQueryOptionsExtensionsTests {
    [Fact]
    public void ToQueryString_KnownProperties_EncodedCorrectly() {
        var opts = new TableQueryOptions {
            Fields = "sys_id",
            Query = "state=1",
            DisplayValue = true,
            ExcludeReferenceLink = true,
            Limit = 5,
            Offset = 10
        };

        var qs = opts.ToQueryString();

        Assert.Equal(
            "sysparm_fields=sys_id&sysparm_query=state%3D1&sysparm_display_value=true&sysparm_exclude_reference_link=true&sysparm_limit=5&sysparm_offset=10",
            qs);
    }

    [Fact]
    public void ToQueryString_AdditionalParameters_AreIncluded() {
        var opts = new TableQueryOptions {
            AdditionalParameters = new Dictionary<string, string?> { ["state"] = "1" }
        };

        Assert.Equal("state=1", opts.ToQueryString());
    }
}
