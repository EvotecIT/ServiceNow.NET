namespace ServiceNow.Configuration;

/// <summary>
/// Represents stored authentication token information.
/// </summary>
public class TokenInfo
{
    /// <summary>Access token value.</summary>
    public string? AccessToken { get; set; }

    /// <summary>Refresh token value.</summary>
    public string? RefreshToken { get; set; }

    /// <summary>Token expiration timestamp.</summary>
    public DateTimeOffset Expires { get; set; }
}
