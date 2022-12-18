using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using webapi.Controllers;
using webapi.Dto;
using webapi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SpengernewsContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));
//builder.Services.AddDbContext<SpengernewsContext>(opt =>
//    opt.UseMySql(
//        builder.Configuration.GetConnectionString("MySql"),
//        new MariaDbServerVersion("10.10.2")));

builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(MappingProfile));

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
// Der Request /api/(controllername) wird so bearbeitet:
// -) Klasse Controllername + Controller wird gesucht.
// -) Controller wird instanziert
// -) Route wird aufgerufen.
// Beispiel: /api/news -> NewsController
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();
