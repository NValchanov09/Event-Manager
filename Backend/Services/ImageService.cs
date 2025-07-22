using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using EventManagerBackend.Helpers;
using EventManagerBackend.Interfaces;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace EventManagerBackend.Services
{
    public class ImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;

        public ImageService(IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
                );

            _cloudinary = new Cloudinary(account);
        }

        public ImageUploadResult UploadImage(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if(file.Length > 0)
            {
                var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                    UploadPreset = "event-manager"
                };

                try
                {
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Upload failed: {ex.Message}");
                }

                stream.Close();
            }

            return uploadResult;
        }

        public DeletionResult DeleteImage(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            return _cloudinary.Destroy(deleteParams);
        }
    }
}
