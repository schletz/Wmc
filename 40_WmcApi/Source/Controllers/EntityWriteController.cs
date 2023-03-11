using AutoMapper;
using Spengernews.Application.Infrastructure;
using WmcApi.Model;

namespace WmcApi.Controllers
{
    public abstract class EntityWriteController<Tentity, Tnewcmd, Teditcmd> : EntityReadController<Tentity> where Tentity : Entity
    {
        protected EntityWriteController(WmcApiContext db, IMapper mapper) : base(db, mapper)
        {
        }
    }
}