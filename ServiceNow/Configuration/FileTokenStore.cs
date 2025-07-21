using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ServiceNow.Utilities;

namespace ServiceNow.Configuration;

/// <summary>
/// Persists tokens to a JSON file on disk.
/// </summary>
public class FileTokenStore : ITokenStore
{
    private readonly string _path;

    /// <summary>
    /// Creates a new <see cref="FileTokenStore"/>.
    /// </summary>
    /// <param name="path">File path used for persistence.</param>
    public FileTokenStore(string path) => _path = path;

    /// <inheritdoc />
    public async Task<TokenInfo?> LoadAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_path))
        {
            return null;
        }

#if NET6_0_OR_GREATER
        var json = await File.ReadAllTextAsync(_path, cancellationToken).ConfigureAwait(false);
#else
        var json = await Task.Run(() => File.ReadAllText(_path), cancellationToken).ConfigureAwait(false);
#endif
        var token = JsonSerializer.Deserialize<TokenInfo>(json, ServiceNowJson.Default);
        return token;
    }

    /// <inheritdoc />
    public async Task SaveAsync(TokenInfo token, CancellationToken cancellationToken)
    {
        var dir = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var json = JsonSerializer.Serialize(token, ServiceNowJson.Default);
#if NET6_0_OR_GREATER
        await File.WriteAllTextAsync(_path, json, cancellationToken).ConfigureAwait(false);
#else
        await Task.Run(() => File.WriteAllText(_path, json), cancellationToken).ConfigureAwait(false);
#endif
    }
}
