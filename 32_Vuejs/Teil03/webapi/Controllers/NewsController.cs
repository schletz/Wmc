using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using webapi.Infrastructure;

namespace webapi.Controllers
{
    [ApiController]
    // [controller] bedeutet: Nimm den Namen vor "Controller"
    // /api/news -> NewsController
    // /api/author -> AuthorController
    // [controller] bedeutet: ASP.NET Core generiert die Route
    // mit dem Wort vor "Controller"
    [Route("/api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly SpengernewsContext _db;

        public NewsController(SpengernewsContext db)
        {
            _db = db;
        }

        public record NewsOverviewDto(int Id, string Headline, string ImageUrl);
        public record NewsDetailDto (int Id, string Headline, string Content);

        // GET Request (app.MapGet("/api/news/")
        [HttpGet]
        public IActionResult GetAllNews()
        {
            string[] images = new string[]
            {
                "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/AA13UCsv.img?w=612&h=304&q=90&m=6&f=jpg&u=t",
                "https://www.bing.com/th?id=ORMS.c64be9536fb2ebb5673dfc61d8142abe&pid=Wdp&w=300&h=156&qlt=90&c=1&rs=1&dpr=1&p=0",
                "https://www.bing.com/th?id=ORMS.805cf20c3f313d9d74bf2cfc96fc7e00&pid=Wdp&w=300&h=156&qlt=90&c=1&rs=1&dpr=1&p=0",
                "https://www.bing.com/th?id=ORMS.430a52f4ed5a6e63b0a376680541e024&pid=Wdp&w=300&h=156&qlt=90&c=1&rs=1&dpr=1&p=0"
            };

            int rowNr = 1000;
            Randomizer.Seed = new Random(1529);
            var news = new Faker<NewsOverviewDto>("de").CustomInstantiator(f =>
            {
                return new NewsOverviewDto(
                    Id: rowNr++,
                    Headline: string.Join(" ", f.Lorem.Words(f.Random.Int(1, 3))),
                    ImageUrl: f.Random.ListItem(images));
            })
            .Generate(10)
            .ToList();
            return Ok(news);
        }

        // GET Request (app.MapGet("/api/news/{id}")
        // z. B. /api/news/17
        // ASP liest die ID aus dem Request und stellt sie unter
        // der Variable id zur Verfügung. Achtung: Nimm genau diese
        // Variable als Parameter der Methode!
        [HttpGet("{id:int}")]
        public IActionResult GetNewsDetail(int id)
        {
            // Um auch 404 Antworten im Frontend zu testen, liefern wir
            // NotFound wenn die id < 1000 ist.
            if (id < 1000) { return NotFound(); }
            Randomizer.Seed = new Random(1529 + id);
            var f = new Faker("de");
            return Ok(new NewsDetailDto(
                Id: id,
                Headline: string.Join(" ", f.Lorem.Words(f.Random.Int(1, 3))),
                Content: f.Lorem.Paragraphs(f.Random.Int(3, 30), "<br />")));
        }
    }
}
