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
    public Task<TokenInfo?> LoadAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_path))
        {
            return Task.FromResult<TokenInfo?>(null);
        }

        var json = File.ReadAllText(_path);
        var token = JsonSerializer.Deserialize<TokenInfo>(json, ServiceNowJson.Default);
        return Task.FromResult(token);
    }

    /// <inheritdoc />
    public Task SaveAsync(TokenInfo token, CancellationToken cancellationToken)
    {
        var dir = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var json = JsonSerializer.Serialize(token, ServiceNowJson.Default);
        File.WriteAllText(_path, json);
        return Task.CompletedTask;
    }
}
