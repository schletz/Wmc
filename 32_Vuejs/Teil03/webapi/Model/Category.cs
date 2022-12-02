﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Model
{
    [Index(nameof(Name), IsUnique = true)]
    public class Category
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

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }
        public string Name { get; set; }
    }


}