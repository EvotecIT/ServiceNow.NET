using System.Threading;
using System.Threading.Tasks;

namespace ServiceNow.Configuration;

/// <summary>
/// Provides persistence for OAuth tokens.
/// </summary>
public interface ITokenStore
{
    /// <summary>Loads the current token from the store.</summary>
    Task<TokenInfo?> LoadAsync(CancellationToken cancellationToken);

    /// <summary>Saves the token to the store.</summary>
    Task SaveAsync(TokenInfo token, CancellationToken cancellationToken);
}
