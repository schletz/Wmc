using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spengernews.Application.Infrastructure;
using System;
using System.Linq;
using WmcApi.Dto;
using WmcApi.Model;

namespace WmcApi.Controllers
{
    public class StudentsController : EntityReadController<Student>
    {
        public StudentsController(WmcApiContext db, IMapper mapper) : base(db, mapper)
        {
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string? @class)
        {
            IQueryable<Student> query = _db.Students;
            if (!string.IsNullOrEmpty(@class)) { query = query.Where(s => s.Class.Shortname == @class); }
            return Query<StudentDto>(query);
        }

        [HttpGet("{guid}")]
        public IActionResult GetById(Guid guid) => Query<StudentDetailsDto>(_db.Students.Include(s => s.Class), s => s.Guid == guid);
    }
}