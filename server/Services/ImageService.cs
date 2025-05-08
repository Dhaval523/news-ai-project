using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Server.Models;
using Server.Helpers;

namespace Server.Services
{
    public interface IImageService
    {
        Task<ApiResponse<string>> UploadImageAsync(IFormFile file);
    }

    public class ImageService : IImageService
    {
        private readonly Cloudinary _cloudinary ;

        public ImageService(IConfiguration configuration)
        {
            var settings = configuration.GetSection("CloudinarySettings");
            var account = new Account(
                settings["CloudName"],
                settings["ApiKey"],
                settings["ApiSecret"]
            );

            _cloudinary = new Cloudinary(account);
           
        }

        public async Task<ApiResponse<string>> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return ApiResponse<string>.FailureResponse("No file uploaded");
            }

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName , stream),
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if(uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return ApiResponse<string>.SuccessResponse(uploadResult.SecureUrl.ToString());
            }

            return ApiResponse<string>.FailureResponse("Cloudinary upload failed");
        }

        
    }
}
