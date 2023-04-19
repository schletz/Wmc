using System;
using System.ComponentModel.DataAnnotations;

namespace Spengernews.Application.Model
{
    // To use as type constraint
    public abstract class Entity
    {
        public Guid Guid { get; set; }
    }
    public abstract class Entity<Tkey> : Entity where Tkey : struct
    {
        [Key]
        public Tkey Id { get; set; }
    }
}