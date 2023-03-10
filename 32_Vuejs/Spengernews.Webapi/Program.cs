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
using Spengernews.Webapi.Services;
using System;
using Webapi;

var builder = WebApplication.CreateBuilder(args);
// *************************************************************************************************
// BUILDER CONFIGURATION
// *************************************************************************************************

// Database********** ******************************************************************************
// Read the sql server connection string from appsettings.json located at
// ConnectionStrings -> Default.
builder.Services.AddDbContext<SpengernewsContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"),
        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)));

// FOR SQLITE:
// builder.Services.AddDbContext<SpengernewsContext>(opt =>
//     opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));
// FOR mariaDB (version 10.10.3):
// builder.Services.AddDbContext<SpengernewsContext>(opt =>
// {
//     opt.UseMySql(
//         builder.Configuration.GetConnectionString("Default"),
//         new MariaDbServerVersion("10.10.3"),
//             o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));
// });
// FOR Postgres:
// builder.Services.AddDbContext<SpengernewsContext>(opt =>
// {
//     opt.UseNpgsql(
//         builder.Configuration.GetConnectionString("Default"),
//             o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));
// });
// // Allow unspecified DateTime values in DateTime Properties.
// AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddHttpContextAccessor();     // Required to access the http context in the auth service.
builder.Services.AddTransient<AuthService>();  // Instantiation on each DI injection.
builder.Services.AddTransient<ArticleService>();

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
        options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    });
}

// *************************************************************************************************
// APP
// *************************************************************************************************
var app = builder.Build();

// SHOW ENVIRONMENT
app.Logger.LogInformation($"ASPNETCORE_ENVIRONMENT is {app.Environment.EnvironmentName}");
app.Logger.LogInformation($"Use Database {builder.Configuration.GetConnectionString("Default")}");

app.UseHttpsRedirection();
if (app.Environment.IsDevelopment())
{
    // We will create a fresh sql server container in development mode. For performance reasons,
    // you can disable deleteAfterShutdown because in development mode the database is deleted
    // before it is generated.
    try
    {
        // For mariaDb or Postgres see comment in WebApplicationDockerExtensions.cs at method UseMariaDbContainer()
        await app.UseSqlServerContainer(
            containerName: "spengernews_sqlserver", version: "latest",
            connectionString: app.Configuration.GetConnectionString("Default"),
            deleteAfterShutdown: true);
    }
    catch (Exception e)
    {
        app.Logger.LogError(e.Message);
        return;
    }
    app.UseCors();
}

// Creating the database.
using (var scope = app.Services.CreateScope())
{
    using (var db = scope.ServiceProvider.GetRequiredService<SpengernewsContext>())
    {
        db.CreateDatabase(isDevelopment: app.Environment.IsDevelopment());
    }
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");
app.Run();