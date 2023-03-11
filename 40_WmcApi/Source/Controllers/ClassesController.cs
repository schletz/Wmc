using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spengernews.Application.Infrastructure;
using System.Linq;
using WmcApi.Dto;
using WmcApi.Model;

namespace WmcApi.Controllers
{
    public class ClassesController : EntityReadController<Schoolclass>
    {
        public ClassesController(WmcApiContext db, IMapper mapper) : base(db, mapper)
        {
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string? department)
        {
            IQueryable<Schoolclass> query = _db.Schoolclasses.OrderBy(s => s.Department).ThenBy(s => s.Shortname);
            if (!string.IsNullOrEmpty(department)) { query = query.Where(s => s.Department == department); }
            return Query<SchoolclassDto>(query);
        }

        [HttpGet("{classname}")]
        public IActionResult GetByName(string classname)
        {
            IQueryable<Schoolclass> query = _db.Schoolclasses.Include(s => s.ClassTeacher);
            query = ExpandQueryByParam(query);
            return Query<SchoolclassDetailsDto>(query, s => s.Shortname == classname);
        }
    }
}