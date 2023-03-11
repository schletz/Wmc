using System;

namespace WmcApi.Model
{
    public abstract class Entity
    {
        public Guid Guid { get; set; }
    }

    public abstract class Entity<Tkey> : Entity
    {
        public Tkey Id { get; set; } = default!;
    }
}