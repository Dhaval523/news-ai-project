using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Server.Models;
using Server.Helpers;
using Server.DTOs;
using Server.Data;

namespace Server.Services
{
    public interface IWorkerService
    {
        Task<ApiResponse<ServiceDTO>> CreateServiceAsync(int userId, WorkerServiceRequest request, IFormFile file);
    }

    public class WorkerService : IWorkerService
    {
        private readonly IImageService _imageService ;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public WorkerService(AppDbContext context , IMapper mapper, IImageService imageService)
        {   
            _context = context;
            _mapper = mapper; 
            _imageService = imageService;
        }

        public async Task<ApiResponse<ServiceDTO>> CreateServiceAsync(int userId, WorkerServiceRequest request, IFormFile file)
        {
            // Upload image first
            var imageResult = await _imageService.UploadImageAsync(file);
            if (!imageResult.Success)
            {
                return ApiResponse<ServiceDTO>.FailureResponse("Image upload failed: " + imageResult.Message);
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
            .FirstOrDefaultAsync();

            var serviceDto = _mapper.Map<ServiceDTO>(result);
            
            return ApiResponse<ServiceDTO>.SuccessResponse(serviceDto, "Service created successfully");
        }


       

        
    }
}