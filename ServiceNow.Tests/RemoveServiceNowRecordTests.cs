#if NET8_0_OR_GREATER
using ServiceNow.PowerShell;
using PS = System.Management.Automation.PowerShell;
using Xunit;

namespace ServiceNow.Tests;

public class RemoveServiceNowRecordTests {
    [Fact]
    public void Cmdlet_Is_Importable_And_Has_Force() {
        using var ps = PS.Create();
        var modulePath = typeof(RemoveServiceNowRecord).Assembly.Location;
        ps.AddCommand("Import-Module").AddParameter("Name", modulePath).Invoke();
        ps.Commands.Clear();
        ps.AddCommand("Get-Command").AddParameter("Name", "Remove-ServiceNowRecord");
        var result = ps.Invoke();
        Assert.NotEmpty(result);
        dynamic cmd = result[0].BaseObject;
        Assert.Contains("Force", cmd.Parameters.Keys);
    }
}
#endif
