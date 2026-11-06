namespace AgriStreamIoT.Services;

public interface IBlobStorageService { Task<string> UploadAsync(IFormFile image, string fieldId, CancellationToken ct = default); }
