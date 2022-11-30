using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using webapi.Controllers;
using webapi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    var opt = new DbContextOptionsBuilder<SpengernewsContext>()
        .UseSqlite(builder.Configuration.GetConnectionString("Sqlite"))
        .Options;

    using (var db = new SpengernewsContext(opt))
    {
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        db.Seed();
    }
}

builder.Services.AddDbContext<SpengernewsContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));
builder.Services.AddControllers();
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
// Der Request /api/(controllername) wird so bearbeitet:
// -) Klasse Controllername + Controller wird gesucht.
// -) Controller wird instanziert
// -) Route wird aufgerufen.
// Beispiel: /api/news -> NewsController
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();
