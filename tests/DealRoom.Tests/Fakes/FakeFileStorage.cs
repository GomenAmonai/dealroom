using DealRoom.Core.Interfaces;

namespace DealRoom.Tests.Fakes;

public class FakeFileStorage : IFileStorage
{
    private readonly Dictionary<string, byte[]> _objects = new();

    public async Task UploadAsync(string key, Stream content, long size, string contentType, CancellationToken ct = default)
    {
        using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, ct);
        _objects[key] = buffer.ToArray();
    }

    public Task<Stream> DownloadAsync(string key, CancellationToken ct = default)
        => Task.FromResult<Stream>(new MemoryStream(_objects[key]));

    public Task DeleteAsync(string key, CancellationToken ct = default)
    {
        _objects.Remove(key);
        return Task.CompletedTask;
    }
}
