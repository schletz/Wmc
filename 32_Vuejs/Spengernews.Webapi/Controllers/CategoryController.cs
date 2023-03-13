using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spengernews.Application.Infrastructure;
using Spengernews.Application.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Spengernews.Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        public record NewCategoryCmd(
            [StringLength(255, MinimumLength = 1, ErrorMessage = "Invalid Name")] string Name);
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

        /// <summary>
        /// Reagiert auf POST /api/category
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddCategory(NewCategoryCmd categoryCmd)
        {
            var category = new Category(name: categoryCmd.Name);
            _db.Categories.Add(category);
            try { await _db.SaveChangesAsync(); }
            catch (DbUpdateException e) { return BadRequest(e.InnerException?.Message ?? e.Message); }
            return CreatedAtAction(nameof(AddCategory), new { category.Guid });

        }
    }
}
