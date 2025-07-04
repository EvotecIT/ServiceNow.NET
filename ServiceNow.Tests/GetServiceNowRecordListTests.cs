#if NET8_0_OR_GREATER
using ServiceNow.PowerShell;
using PS = System.Management.Automation.PowerShell;
using Xunit;

namespace ServiceNow.Tests;

public class GetServiceNowRecordListTests {
    [Fact]
    public void Cmdlet_Is_Importable() {
        using var ps = PS.Create();
        var modulePath = typeof(GetServiceNowRecordList).Assembly.Location;
        ps.AddCommand("Import-Module").AddParameter("Name", modulePath).Invoke();
        ps.Commands.Clear();
        ps.AddCommand("Get-Command").AddParameter("Name", "Get-ServiceNowRecordList");
        var result = ps.Invoke();
        Assert.NotEmpty(result);
    }
}
#endif
