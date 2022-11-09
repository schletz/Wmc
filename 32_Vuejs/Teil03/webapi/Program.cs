using System.Reflection.Metadata.Ecma335;
using webapi.Controllers;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Liefert alle News mit ID, BildURL, Headline
app.MapGet("/api/news", () => new NewsController().GetAllNews());
// Liefert den Content der News (id)
app.MapGet("/api/news/{id}", (int id) => new NewsController().GetNewsDetail(id));

app.Run();