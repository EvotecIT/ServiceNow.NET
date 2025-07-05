using System.Threading;
using System.Threading.Tasks;
using ServiceNow.Configuration;

namespace ServiceNow.Tests;

public class InMemoryTokenStore : ITokenStore
{
    public TokenInfo? Token { get; set; }

    public Task<TokenInfo?> LoadAsync(CancellationToken cancellationToken) => Task.FromResult(Token);

    public Task SaveAsync(TokenInfo token, CancellationToken cancellationToken)
    {
        Token = token;
        return Task.CompletedTask;
    }
}
