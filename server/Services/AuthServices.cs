using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity ;
using Microsoft.EntityFrameworkCore ;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using  Server.Models;
using Server.Data ;
using Server.DTOs;
using Server.Helpers; 


namespace Server.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<object>> RegisterAsync(RegisterRequest request);
        Task<ApiResponse<object>> LoginAsync(LoginRequest request);
    }

    public class AuthService : IAuthService
    {   
        
        private readonly AppDbContext _context;  
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(AppDbContext context,  IPasswordHasher<User> passwordHasher , IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<object>> RegisterAsync(RegisterRequest request)
        {
             try
            {   
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

                if (user != null)
                {   
                    return ApiResponse<object>.FailureResponse("User already exists");
                }


                var newUser = new User
                {
                    FullName = request.FullName,
                    Email = request.Email ,
                    Role = request.Role
                };

                newUser.PasswordHash = _passwordHasher.HashPassword(newUser, request.Password);
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                var resultData = new
                {
                    UserId = newUser.UserId,
                    Email = newUser.Email ,
                    Role = newUser.Role
                };

                return ApiResponse<object>.SuccessResponse(resultData , "User registered successfully");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while registering user: " + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("Inner exception: " + ex.InnerException.Message);
                throw;
            }
        }

        public async Task<ApiResponse<object>> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

                if (user == null)
                {
                    return ApiResponse<object>.FailureResponse("Invalid Email or Password.");
                }

                var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

                if (verificationResult == PasswordVerificationResult.Failed)
                {
                    return ApiResponse<object>.FailureResponse("Invalid Email or Password.");
                }

                var claims = new List<Claim>
                {
                    new Claim("UserId", user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                user.LastLogin = DateTime.UtcNow;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                
                var resultData = new
                {
                    UserId = user.UserId,
                    Email = user.Email ,
                    Role = user.Role
                };

                return ApiResponse<object>.SuccessResponse(resultData, "Login successful");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.FailureResponse($"An error occurred: {ex.Message}");
            }
        }

    }
}
