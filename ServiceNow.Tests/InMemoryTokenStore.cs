using System.Threading;
using System.Threading.Tasks;
using ServiceNow.Configuration;

namespace ServiceNow.Tests;

/// <summary>
/// Simple in-memory implementation of <see cref="ITokenStore"/> used for tests.
/// </summary>
public class InMemoryTokenStore : ITokenStore
{
    /// <summary>
    /// Stored token information.
    /// </summary>
    public TokenInfo? Token { get; set; }

    /// <inheritdoc />
    public Task<TokenInfo?> LoadAsync(CancellationToken cancellationToken) => Task.FromResult(Token);

    /// <inheritdoc />
    public Task SaveAsync(TokenInfo token, CancellationToken cancellationToken)
    {
        Token = token;
        return Task.CompletedTask;
    }
}
