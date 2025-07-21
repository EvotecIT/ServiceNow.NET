using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ServiceNow.Configuration;
using Xunit;

namespace ServiceNow.Tests;

public class FileTokenStoreTests
{
    [Fact]
    public async Task LoadAsync_NoFile_ReturnsNull()
    {
        var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var store = new FileTokenStore(path);

        var result = await store.LoadAsync(CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task SaveAsync_ThenLoadAsync_RoundTripsToken()
    {
        var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        try
        {
            var store = new FileTokenStore(path);
            var token = new TokenInfo
            {
                AccessToken = "a",
                RefreshToken = "r",
                Expires = DateTimeOffset.UtcNow.AddMinutes(1)
            };

            await store.SaveAsync(token, CancellationToken.None);

            var result = await store.LoadAsync(CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(token.AccessToken, result!.AccessToken);
            Assert.Equal(token.RefreshToken, result.RefreshToken);
            Assert.Equal(token.Expires, result.Expires);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
