using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Server.Services;
using Server.Data;
using Server.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "KaamWale";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
        options.LoginPath = "/api/auth/login";
    })
    .AddGoogle(options =>
    {
        var googleAuthSection = builder.Configuration.GetSection("Authentication:Google");
        options.ClientId = googleAuthSection["ClientId"];
        options.ClientSecret = googleAuthSection["ClientSecret"];
        options.Events.OnRemoteFailure = context =>
        {
            context.Response.Redirect("/auth/error?message=" + Uri.EscapeDataString(context.Failure?.Message));
            context.HandleResponse(); // suppress the exception
            return Task.CompletedTask;
        };
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();




builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/auth/error", (HttpContext http) =>
{
    var message = http.Request.Query["message"];
    return Results.Text($"OAuth error: {message}");
});

app.UseAuthentication();  
app.UseAuthorization();
app.MapControllers();
app.Run();