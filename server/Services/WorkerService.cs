using Microsoft.EntityFrameworkCore;
using Server.Models;
using Server.Helpers;
using Server.DTOs;
using Server.Data;

namespace Server.Services
{
    public interface IWorkerService
    {
        Task<ApiResponse<object>> CreateServiceAsync(int userId, WorkerServiceRequest request, IFormFile file);
    }

    public class WorkerService : IWorkerService
    {
        private readonly IImageService _imageService ;
        private readonly AppDbContext _context;

        public WorkerService(AppDbContext context , IImageService imageService)
        {   
            _context = context;
            _imageService = imageService;
        }

        public async Task<ApiResponse<object>> CreateServiceAsync(int userId, WorkerServiceRequest request, IFormFile file)
        {
            // Upload image first
            var imageResult = await _imageService.UploadImageAsync(file);
            if (!imageResult.Success)
            {
                return ApiResponse<object>.FailureResponse("Image upload failed: " + imageResult.Message);
            }

            // Map request to entity
            ServiceTable service = new ServiceTable
            {
                WorkerId = userId,
                Title = request.Title,
                Description = request.Description,
                Price = request.Price,
                ImageUrl = imageResult.Data,
                ExperienceInYears = request.ExperienceInYears,
                Category = request.Category,
                CreatedAt = DateTime.UtcNow
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            var result = await _context.Services
            .Include(ws => ws.Worker)
            .Where(ws => ws.Id == service.Id)
            .Select(ws => new 
            {
                Id = ws.Id,
                Title = ws.Title,
                Price = ws.Price,
                ImageUrl = ws.ImageUrl,
                Description = ws.Description,
                TotalEarnings = ws.TotalEarnings ,
                TotalJobsCompleted = ws.TotalJobsCompleted ,
                ExperienceInYears = ws.ExperienceInYears ,
                Rating = ws.Rating ,
                Category = ws.Category,
                IsAvailable = ws.IsAvailable ,
                Worker = new 
                {
                    UserId = ws.Worker.UserId,
                    FullName = ws.Worker.FullName,
                    Email = ws.Worker.Email,
                    ProfileImageUrl = ws.Worker.ProfileImageUrl,
                    Bio = ws.Worker.Bio ,
                    Role = ws.Worker.Role ,
                    Location = ws.Worker.Location ,
                    IsVerified = ws.Worker.IsVerified ,
                    CreatedAt = ws.Worker.CreatedAt ,
                    Preferences = ws.Worker.Preferences
                }
            })
            .FirstOrDefaultAsync();
            
            return ApiResponse<object>.SuccessResponse(result, "Service created successfully");
        }


       

        
    }
}