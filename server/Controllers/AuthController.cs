using System.Text.Json ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
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
        private readonly IConfiguration _configuration;
        
        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
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

        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromBody] LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);

            if (!response.Success)
            {
                return BadRequest(response); 
            }

            return Ok(response);
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleLoginResponse", "Auth");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("GoogleLoginResponse")]
        public async Task<IActionResult> GoogleLoginResponse()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            var response = await _authService.GoogleLoginResponseAsync(email);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            // Serialize the response data to JSON
            var json = JsonSerializer.Serialize(response);

            var data = JsonSerializer.Serialize(response.Data);

            // Deserialize into JsonElement
            JsonElement userInfo = JsonSerializer.Deserialize<JsonElement>(data);


            // Get the "Role" property
            string role = userInfo.GetProperty("Role").GetString();
            string baseUrl = _configuration["FrontEndBaseUrl"];

            // Redirect based on role
            if (role == "worker")
            {
                return Redirect($"{baseUrl}/workerdashboard");
            }
            else
            {
                return Redirect($"{baseUrl}/userdashboard");
            }
        }

        [HttpGet("google-signup")]
        public IActionResult GoogleSignUp([FromQuery] string role)
        {
            var redirectUrl = Url.Action("SignUpResponse", "Auth");
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };

            // Store role in authentication properties
            properties.Items["role"] = role;

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }


        [HttpGet("SignUpResponse")]
        public async Task<IActionResult> SignUpResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var fullName = User.FindFirst(ClaimTypes.Name)?.Value;
            var picture = User.FindFirst("urn:google:picture")?.Value;
            var role = result.Properties?.Items["role"];


            var response = await _authService.GoogleSignUpResponseAsync(fullName , email ,  role ,  picture);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            // Serialize the response data to JSON
            var json = JsonSerializer.Serialize(response);

            var data = JsonSerializer.Serialize(response.Data);

            // Deserialize into JsonElement
            JsonElement userInfo = JsonSerializer.Deserialize<JsonElement>(data);


            // Get the "Role" property
            string userRole = userInfo.GetProperty("Role").GetString();
            string baseUrl = _configuration["FrontEndBaseUrl"];

            // Redirect based on role
            if (userRole == "worker")
            {
                return Redirect($"{baseUrl}/workerdashboard");
            }
            else
            {
                return Redirect($"{baseUrl}/userdashboard");
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { Message = "User is not authenticated." });
            }

            // Use the correct custom claim key ("UserId")
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return BadRequest(new { Message = "User ID not found in claims." });
            }

            var user = await _authService.GetUser(userIdClaim);

            if (!user.Success)
            {
                return NotFound(user);
            }

            return Ok(user);
        }
        
        [HttpGet("log-out")]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Logout Successully");
        }





    }
}
