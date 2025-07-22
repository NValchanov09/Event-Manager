using CloudinaryDotNet.Actions;

namespace EventManagerBackend.Interfaces
{
    public interface IImageService
    {
        ImageUploadResult UploadImage(IFormFile file);
        DeletionResult DeleteImage(string publicId);
    }
}
