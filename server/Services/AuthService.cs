using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity ;
using Microsoft.EntityFrameworkCore ;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Cryptography;
using System.Security.Claims;
using  Server.Models;
using Server.Data ;
using Server.DTOs;
using Server.Helpers; 
using AutoMapper;


namespace Server.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<object>> RegisterAsync(RegisterRequest request);
        Task<ApiResponse<object>> LoginAsync(LoginRequest request);
        Task<ApiResponse<object>> GoogleLoginResponseAsync(string email);
        Task<ApiResponse<object>> GoogleSignUpResponseAsync(string fullName , string email , string role , string imageUrl);
        Task<ApiResponse<UserDTO>> GetUser(string id);
    }

    public class AuthService : IAuthService
    {   
        
        private readonly AppDbContext _context;  
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper ;

        public AuthService(AppDbContext context,  IPasswordHasher<User> passwordHasher , IHttpContextAccessor httpContextAccessor , IMapper mapper)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
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

                await GenerateTokenAsync(user);
                

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

        public async Task<ApiResponse<object>> GoogleLoginResponseAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return  ApiResponse<object>.FailureResponse("User does not exist.");
            }

            await GenerateTokenAsync(user);
                

            user.LastLogin = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

                
            var resultData = new
            {
                UserId = user.UserId,
                Email = user.Email ,
                Role = user.Role
            };

            return  ApiResponse<object>.SuccessResponse(resultData, "Login successful");

        }

        public async Task<ApiResponse<object>> GoogleSignUpResponseAsync(string fullName , string email , string role , string imageUrl)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
               return await GoogleLoginResponseAsync(email);
            }
            
            var password = GenerateSecurePassword();
            var newUser = new User
            {
                FullName = fullName,
                Email = email ,
                Role = role ,
                ProfileImageUrl = imageUrl 
            };
            var hashedPassword = _passwordHasher.HashPassword(newUser, password);

            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, password);
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return await GoogleLoginResponseAsync( newUser.Email);

        }

        public async Task<ApiResponse<UserDTO>> GetUser(string id)
        {
            if (!int.TryParse(id, out int userId))
            {
                return ApiResponse<UserDTO>.FailureResponse("Invalid user ID format.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return ApiResponse<UserDTO>.FailureResponse("User not found.");
            }

            var userInfo = _mapper.Map<UserDTO>(user);

            return ApiResponse<UserDTO>.SuccessResponse(userInfo, "Fetch user successfully.");
        }


        private async Task GenerateTokenAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("UserId", user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        private static string GenerateSecurePassword(int length = 32)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
            var password = new char[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] bytes = new byte[length];
                rng.GetBytes(bytes);
                for (int i = 0; i < length; i++)
                {
                    password[i] = validChars[bytes[i] % validChars.Length];
                }
            }
            return new string(password);
        }

    }
}
