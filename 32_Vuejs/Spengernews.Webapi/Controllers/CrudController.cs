using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spengernews.Application.Infrastructure;
using Spengernews.Application.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Spengernews.Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class CrudController<TEntity> : ControllerBase where TEntity : Entity
    {
        protected readonly SpengernewsContext _db;
        protected readonly IMapper _mapper;

        public CrudController(SpengernewsContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IActionResult> GetAll<TDto>()
        {
            return Ok(await _mapper.ProjectTo<TDto>(_db.Set<TEntity>()).ToListAsync());
        }

        public async Task<IActionResult> GetById<TDto>(Guid guid)
        {
            return Ok(await _mapper.ProjectTo<TDto>(
                _db.Set<TEntity>().Where(e => e.Guid == guid))
                .FirstOrDefaultAsync());
        }

        public async Task<IActionResult> Update<TCmd>(Guid guid, TCmd updateCmd)
        {
            var found = await _db.Set<TEntity>()
                .FirstOrDefaultAsync(e => e.Guid == guid);
            if (found is null) { return NotFound(); }
            _mapper.Map(updateCmd, found);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                return BadRequest(e.InnerException?.Message ?? e.Message);
            }
            return NoContent();
        }

        public async Task<IActionResult> Add<TCmd>(TCmd addCmd)
        {
            var entity = _mapper.Map<TEntity>(addCmd);
            await _db.AddAsync(entity);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                return BadRequest(e.InnerException?.Message ?? e.Message);
            }
            return NoContent();
        }

        public async Task<IActionResult> Delete(Guid guid)
        {
            var found = await _db.Set<TEntity>()
                .FirstOrDefaultAsync(e => e.Guid == guid);
            if (found is null) { return NotFound(); }
            _db.Remove(found);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                return BadRequest(e.InnerException?.Message ?? e.Message);
            }
            return NoContent();
        }

        public async Task<IActionResult> Query<TDto>(IQueryable<TEntity> query)
        {
            return Ok(await _mapper.ProjectTo<TDto>(query).ToListAsync());
        }
        public async Task<IActionResult> Query<T>(IQueryable<T> query)
        {
            return Ok(await query.ToListAsync());
        }
    }
}
