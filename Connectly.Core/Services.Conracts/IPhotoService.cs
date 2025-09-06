namespace Connectly.Core.Services.Conracts;

public interface IPhotoService
{
   Task<ImageUploadResult> UploadPhotoAsync(IFormFile file);
   Task<DeletionResult> DeletePhotoAsync(string publicId);
}
