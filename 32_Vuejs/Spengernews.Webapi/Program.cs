using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Spengernews.Application.Dto;
using Spengernews.Application.Infrastructure;
using System;
using Webapi;

var builder = WebApplication.CreateBuilder(args);
// *************************************************************************************************
// BUILDER CONFIGURATION
// *************************************************************************************************
//builder.Services.AddDbContext<SpengernewsContext>(opt =>
//    opt.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));

// Read the sql server connection string from appsettings.json. A container for this connection string
// is automatically started in development mode.
builder.Services.AddDbContext<SpengernewsContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(MappingProfile));

// JWT Authentication ******************************************************************************
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.IdentityModel.Tokens;

byte[] secret = Convert.FromBase64String(builder.Configuration["Secret"]);
builder.Services
    .AddAuthentication(options => options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secret),
            ValidateAudience = false,
            ValidateIssuer = false
        };
    });
// *************************************************************************************************

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(
            builder =>
            {
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
    });
}

// *************************************************************************************************
// APP
// *************************************************************************************************
var app = builder.Build();
app.UseHttpsRedirection();
if (app.Environment.IsDevelopment())
{
    // We create a fresh sql server container in development mode. Therefore we do not need EnsureDeleted().
    // For performance reasons you can disable deleteAfterShutdown and call EnsureCreated().
    try
    {
        await app.UseSqlServerContainer(
            containerName: "spengernews_sqlserver",
            connectionString: app.Configuration.GetConnectionString("SqlServer"),
            deleteAfterShutdown: true);
    }
    catch (Exception e)
    {
        app.Logger.LogError(e.Message);
        return;
    }
    using (var scope = app.Services.CreateScope())
    {
        using (var db = scope.ServiceProvider.GetRequiredService<SpengernewsContext>())
        {
            //db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            db.Seed();
        }
    }
    app.UseCors();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");
app.Run();
