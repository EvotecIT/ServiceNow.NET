using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using ServiceNow.Enums;
using ServiceNow.Extensions;
using Xunit;

namespace ServiceNow.Tests;

public class EnumExtensionsTests {
    [Theory]
    [InlineData(IncidentState.New, "New")]
    [InlineData(IncidentState.InProgress, "In Progress")]
    [InlineData(IncidentState.OnHold, "On Hold")]
    [InlineData(IncidentState.Resolved, "Resolved")]
    [InlineData(IncidentState.Closed, "Closed")]
    [InlineData(IncidentState.Canceled, "Canceled")]
    public void IncidentState_ToDisplayString_ReturnsExpected(IncidentState state, string expected) {
        var result = state.ToDisplayString();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(ServiceNowRole.Admin, "Admin")]
    [InlineData(ServiceNowRole.ITIL, "ITIL")]
    [InlineData(ServiceNowRole.Approver, "Approver")]
    [InlineData(ServiceNowRole.User, "User")]
    public void ServiceNowRole_ToDisplayString_ReturnsExpected(ServiceNowRole role, string expected) {
        var result = role.ToDisplayString();
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToDisplayString_CachePerformance() {
        var field = typeof(EnumExtensions).GetField("_displayNameCache", BindingFlags.NonPublic | BindingFlags.Static)!;
        var cache = (ConcurrentDictionary<Type, Dictionary<Enum, string>>)field.GetValue(null)!;
        cache.Clear();

        _ = IncidentState.New.ToDisplayString();
        var countAfterFirst = cache.Count;

        _ = IncidentState.Resolved.ToDisplayString();
        var countAfterSecond = cache.Count;

        Assert.Equal(1, countAfterFirst);
        Assert.Equal(countAfterFirst, countAfterSecond);

        const int iterations = 200000;
        var sw1 = Stopwatch.StartNew();
        for (var i = 0; i < iterations; i++) {
            _ = IncidentState.New.ToDisplayString();
        }
        sw1.Stop();

        var sw2 = Stopwatch.StartNew();
        for (var i = 0; i < iterations; i++) {
            _ = IncidentState.New.ToDisplayString();
        }
        sw2.Stop();

        Assert.True(sw2.ElapsedMilliseconds < sw1.ElapsedMilliseconds);
    }

}
