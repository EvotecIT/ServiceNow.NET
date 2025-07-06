#if NET8_0_OR_GREATER || NETFRAMEWORK
using ServiceNow.PowerShell;
using PS = System.Management.Automation.PowerShell;
using Xunit;

namespace ServiceNow.Tests;

public class FindServiceNowKnowledgeTests {
    [Fact]
    public void Cmdlet_Is_Importable() {
        using var ps = PS.Create();
        var modulePath = typeof(FindServiceNowKnowledge).Assembly.Location;
        ps.AddCommand("Import-Module").AddParameter("Name", modulePath).Invoke();
        ps.Commands.Clear();
        ps.AddCommand("Get-Command").AddParameter("Name", "Find-ServiceNowKnowledge");
        var result = ps.Invoke();
        Assert.NotEmpty(result);
    }
}
#endif
