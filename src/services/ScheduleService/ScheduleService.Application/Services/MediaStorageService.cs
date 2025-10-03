using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleService.Application.IServices;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace ScheduleService.Application.Services
{
    public class MediaStorageService : IMediaStorageService
    {
        private readonly Cloudinary _cloudinary;
        public MediaStorageService()
        {
            var cloud = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME");
            var key = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY");
            var secret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET");
            _cloudinary = new Cloudinary(new Account(cloud, key, secret));
        }

        public async Task<string> SaveAsync(IFormFile file)
        {
            await using var stream = file.OpenReadStream();
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var isVideo = file.ContentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase)
                          || new[] { ".mp4", ".mov", ".avi", ".mkv" }.Contains(ext);

            if (isVideo)
            {
                var upload = new VideoUploadParams { File = new FileDescription(file.FileName, stream) };
                var result = await _cloudinary.UploadAsync(upload);
                return result?.SecureUrl?.ToString() ?? throw new Exception("Video upload failed");
            }
            else
            {
                var upload = new ImageUploadParams { File = new FileDescription(file.FileName, stream) };
                var result = await _cloudinary.UploadAsync(upload);
                return result?.SecureUrl?.ToString() ?? throw new Exception("Image upload failed");
            }
        }
    }
}
