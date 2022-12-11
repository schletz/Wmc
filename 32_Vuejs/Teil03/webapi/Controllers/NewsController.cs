using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using webapi.Infrastructure;

namespace webapi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly SpengernewsContext _db;

        public NewsController(SpengernewsContext db)
        {
            _db = db;
        }

        /// <summary>
        /// GET Request (app.MapGet("/api/news/")
        /// </summary>
        [HttpGet]
        public IActionResult GetAllNews()
        {
            // Use ToList to read from database. If not, the serializer
            // wants to serialize all your navigations!
            var news = _db.Articles.OrderBy(a => a.Created).ToList();
            return Ok(news);
        }

        /// <summary>
        /// GET Request (app.MapGet("/api/news/{id}")
        /// </summary>
        [HttpGet("{id:int}")]
        public IActionResult GetNewsDetail(int id)
        {
            var article = _db.Articles.FirstOrDefault(a => a.Id == id);
            if (article is null) { return NotFound(); }
            return Ok(article);
        }
    }
}
