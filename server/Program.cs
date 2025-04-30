using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Server.Data;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));



builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();