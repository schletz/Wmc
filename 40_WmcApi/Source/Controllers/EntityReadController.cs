using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spengernews.Application.Infrastructure;
using System;
using System.Linq;
using System.Linq.Expressions;
using WmcApi.Model;

namespace WmcApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public abstract class EntityReadController<Tentity> : ControllerBase where Tentity : Entity
    {
        protected readonly WmcApiContext _db;
        protected readonly IMapper _mapper;

        protected EntityReadController(WmcApiContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        protected IActionResult Query<Tdto>(IQueryable<Tentity> query)
        {
            var result = _mapper.ProjectTo<Tdto>(query.AsNoTracking()).ToList();
            return Ok(result);
        }

        protected IActionResult Query<Tdto>(IQueryable<Tentity> query, Expression<Func<Tentity, bool>> predicate)
        {
            var result = _mapper.Map<Tdto>(query.AsNoTracking().FirstOrDefault(predicate));
            if (result is null) { return NotFound(); }
            return Ok(result);
        }

        protected IQueryable<Tentity> ExpandQueryByParam(IQueryable<Tentity> query)
        {
            var entity = _db.Model.FindEntityType(typeof(Tentity));
            if (entity is null) { throw new ApplicationException($"Entity {typeof(Tentity).Name} not found."); }
            if (!HttpContext.Request.Query.TryGetValue("$expand", out var paramValues))
                return query;
            var values = paramValues.SelectMany(v => v.Split(",")).ToList();

            var expandNavigations = entity.GetNavigations()
                .Where(n => values.Contains(n.Name.ToLower())).Select(n => n.Name);
            foreach (var navigation in expandNavigations)
                query = query.Include(navigation);
            return query;
        }
    }
}