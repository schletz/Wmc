using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spengernews.Application.Infrastructure;
using Spengernews.Application.Model;
using System.Linq;
using System.Threading.Tasks;

namespace Spengernews.Webapi.Controllers
{
    public class AuthorsController : CrudController<Author>
    {
        public AuthorsController(SpengernewsContext db, IMapper mapper) : base(db, mapper)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAuthors() =>
            await Query(_db.Authors.Select(a => new { a.Guid, a.Firstname, a.Lastname }));
    }
}
