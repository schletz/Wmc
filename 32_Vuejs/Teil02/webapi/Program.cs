var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// SPA fragt nach /news/{id}
app.MapGet("/news/{id}", (int id) => new
{
    Url= "https://assets.orf.at/mims/2022/40/23/crops/w=171,q=90,r=1/1514150_1k_550823_wirtschaft_erfolgreiche_laender_zko.jpg?s=8c6cb33bc7f34b3ae5eacb249b8e75a0831a89b8",
    Headline=$"Headline zu {id}"
});

app.Run();