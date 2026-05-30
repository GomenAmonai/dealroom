namespace DealRoom.Core.Interfaces;

public interface IFileStorage
{
    Task UploadAsync(string key, Stream content, long size, string contentType, CancellationToken ct = default);
    Task<Stream> DownloadAsync(string key, CancellationToken ct = default);
    Task DeleteAsync(string key, CancellationToken ct = default);
}
