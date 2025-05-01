using Microsoft.AspNetCore.Mvc;


using System.Security.Claims;
using Server.Services;
using Server.Models;
using Server.DTOs;
namespace Server.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {   
        private readonly IAuthService _authService ;
        
        public AuthController(IAuthService authService){
            _authService = authService ;
        }

        [HttpPost("signUp")]
        public async Task<IActionResult> SignUp([FromBody] RegisterRequest request)
        {
            try
            {
                var response = await _authService.RegisterAsync(request);

                if(!response.Success){
                    return BadRequest(response);
                }
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpPost("signIn")]
        public async Task<IActionResult> SignIn([FromBody] LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);

            if (!response.Success)
            {
                return BadRequest(response); // You can add more specific error handling here
            }

            return Ok(response);
        }



    }
}
