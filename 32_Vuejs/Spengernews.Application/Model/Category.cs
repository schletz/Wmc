using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spengernews.Application.Model
{
    [Index(nameof(Name), IsUnique = true)]
    public class Category : Entity<int>
    {
#pragma warning disable CS8618
        protected Category() { }
#pragma warning restore CS8618 
        // Auto increment Werte fordern wir nicht im Konstruktor an,
        // da sie erst in der DB entstehen!
        public Category(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }


}
