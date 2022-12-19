using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Spengernews.Application.Dto;
using Spengernews.Application.Infrastructure;
using System;

var builder = WebApplication.CreateBuilder(args);
// *************************************************************************************************
// BUILDER CONFIGURATION
// *************************************************************************************************
builder.Services.AddDbContext<SpengernewsContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));
//builder.Services.AddDbContext<SpengernewsContext>(opt =>
//    opt.UseMySql(
//        builder.Configuration.GetConnectionString("MySql"),
//        new MariaDbServerVersion("10.10.2")));

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
// Leitet http auf https weiter (http Port 5000 auf https Port 5001)
app.UseHttpsRedirection();
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        using (var db = scope.ServiceProvider.GetRequiredService<SpengernewsContext>())
        {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            db.Seed();
        }
    }
    app.UseCors();
}
app.UseStaticFiles();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.UseAuthentication();
app.UseAuthorization();
app.Run();
