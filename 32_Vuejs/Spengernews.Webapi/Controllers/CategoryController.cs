using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spengernews.Application.Infrastructure;
using System.Linq;

namespace Spengernews.Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly SpengernewsContext _db;

        public CategoryController(SpengernewsContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetAllCategories()
        {
            return Ok(_db.Categories.Select(c => new
            {
                c.Guid,
                c.Name
            })
            .ToList());
        }
    }
}
