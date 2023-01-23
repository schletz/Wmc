using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using WebsocketDemo.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

builder.Services.AddSingleton(opt => new WebsocketService(opt.GetRequiredService<ILogger<WebsocketService>>()));
builder.Services.AddControllers();
builder.WebHost.UseUrls("https://localhost:5001");

var app = builder.Build();
// Middleware for websocket connection. The request is processed in WebsocketController.
app.UseWebSockets(new WebSocketOptions { KeepAliveInterval = TimeSpan.FromMinutes(2) });
app.UseHttpsRedirection();
if (app.Environment.IsDevelopment())
{
    app.UseCors();
}
app.UseStaticFiles();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();