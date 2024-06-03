using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Cloudery
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(string cloudName, string apiKey, string apiSecret)
        {
            Account account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null)
            {
                return null;
            }

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream)
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl?.AbsoluteUri;
            }
        }
        public async Task<string> UploadImageAsync(string filePath)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(filePath)
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl?.AbsoluteUri;
        }
        public async Task<string> UploadBase64ImageAsync(string base64Image)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription($"data:image/png;base64,{base64Image}")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return uploadResult.Url.ToString();
            }

            throw new Exception($"Error uploading image: {uploadResult.Error.Message}");
        }
        public async Task<string> UploadVideoAsync(IFormFile file)
        {
            // Ensure the file is not null
            if (file == null)
            {
                return null;
            }

            // Open a stream for the file
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new VideoUploadParams()
                {
                    File = new FileDescription(file.FileName, stream)
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl?.AbsoluteUri;
            }
        }
        public async Task<string> UploadVideoFromBase64Async(string base64String)
        {
            var fileName = $"video_{Guid.NewGuid()}.mp4";
            var uploadParams = new VideoUploadParams()
            {
                File = new FileDescription(fileName, new MemoryStream(Convert.FromBase64String(base64String)))
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl?.AbsoluteUri;
        }

    }
}
