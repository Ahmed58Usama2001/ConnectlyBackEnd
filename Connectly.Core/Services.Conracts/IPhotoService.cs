
namespace Connectly.Core.Services.Conracts;

public interface IPhotoService
{
    Task<PhotoUploadResult> UploadPhotoAsync(Stream fileStream, string fileName);
    Task<PhotoDeletionResult> DeletePhotoAsync(string publicId);
}
