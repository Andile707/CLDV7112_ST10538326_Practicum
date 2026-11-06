using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
namespace AgriStreamIoT.Services;

public class BlobStorageService(BlobServiceClient blobs) : IBlobStorageService
{
    public async Task<string> UploadAsync(IFormFile image, string fieldId, CancellationToken ct = default)
    {
        if (image.Length == 0 || image.Length > 10 * 1024 * 1024) throw new ArgumentException("Image must be 1 byte to 10 MB.");
        var allowed = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { [".jpg"] = "image/jpeg", [".jpeg"] = "image/jpeg", [".png"] = "image/png" };
        var ext = Path.GetExtension(image.FileName);
        if (!allowed.TryGetValue(ext, out var mime) || !string.Equals(image.ContentType, mime, StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("Only JPEG and PNG images are accepted.");
        if (string.IsNullOrWhiteSpace(fieldId) || fieldId.Length > 100) throw new ArgumentException("Invalid field ID.");
        var container = blobs.GetBlobContainerClient("drone-images");
        await container.CreateIfNotExistsAsync(cancellationToken: ct);
        var name = $"{Guid.NewGuid():N}{ext.ToLowerInvariant()}";
        var blob = container.GetBlobClient(name);
        await using var stream = image.OpenReadStream();
        await blob.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = mime }, Metadata = new Dictionary<string, string> { ["fieldid"] = fieldId } }, ct);
        return name;
    }
}
