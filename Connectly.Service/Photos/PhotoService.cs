
namespace Connectly.Service.Photos;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;

    public PhotoService(IOptions<CloudinarySettings> settings)
    {
        var account = new Account(
            settings.Value.CloudName,
            settings.Value.ApiKey,
            settings.Value.ApiSecret);

        _cloudinary = new Cloudinary(account);
    }

    public async Task<PhotoUploadResult> UploadPhotoAsync(Stream fileStream, string fileName)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, fileStream)
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        return new PhotoUploadResult
        {
            PublicId = result.PublicId,
            Url = result.Url.ToString()
        };
    }

    public async Task<PhotoDeletionResult> DeletePhotoAsync(string publicId)
    {
        var deletionParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deletionParams);

        return new PhotoDeletionResult
        {
            Success = result.Result == "ok"
        };
    }
}