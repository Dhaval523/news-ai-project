using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Server.Services ;
using Server.DTOs;

namespace Server.Controller
{   
    
    [ApiController]
    [Route("api/[controller]")]
    public class WorkerController : ControllerBase
    {   
        private readonly IImageService _imageService ;
        private readonly IWorkerService _workerService ;

        public WorkerController(IImageService imageService , IWorkerService workerService)
        {
            _imageService = imageService;
            _workerService = workerService ;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload( [FromForm] IFormFile file)
        {   
            var result = await _imageService.UploadImageAsync(file);
            return Ok(result);
        }

        [Authorize(Roles = "worker")] 
        [HttpPost("create-service")]
        public async Task<IActionResult> CreateService([FromForm] WorkerServiceRequest request, [FromForm] IFormFile image)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user ID in claims." });
            }

            var response = await _workerService.CreateServiceAsync(userId, request, image);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

       
      
       


        
        





    }
}
