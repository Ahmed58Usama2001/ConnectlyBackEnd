namespace Connectly.Core.Dtos;

public class PhotoUploadResult
{
    public string PublicId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

public class PhotoDeletionResult
{
    public bool Success { get; set; }
}
