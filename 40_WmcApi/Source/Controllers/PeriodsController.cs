using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Spengernews.Application.Infrastructure;
using System.Linq;
using WmcApi.Dto;
using WmcApi.Model;

namespace WmcApi.Controllers
{
    public class PeriodsController : EntityReadController<Period>
    {
        public PeriodsController(WmcApiContext db, IMapper mapper) : base(db, mapper)
        {
        }

        [HttpGet] public IActionResult GetAll() => Query<PeriodDto>(_db.Periods.OrderBy(p => p.Nr));
    }
}