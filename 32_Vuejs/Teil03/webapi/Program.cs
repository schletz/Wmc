using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using webapi.Controllers;

var builder = WebApplication.CreateBuilder(args);
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

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseCors();
}
app.UseStaticFiles();

// Liefert alle News mit ID, BildURL, Headline
app.MapGet("/api/news", () => new NewsController().GetAllNews());
// Liefert den Content der News (id)
app.MapGet("/api/news/{id}", (int id) => new NewsController().GetNewsDetail(id));

app.MapFallbackToFile("index.html");
app.Run();