using Microsoft.EntityFrameworkCore;
using System;

namespace WmcApi.Model
{
    [Index(nameof(Shortname), IsUnique = true)]
    public class Room : Entity<int>
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected Room()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        { }

        public Room(string shortname, string info, int capacity)
        {
            Shortname = shortname;
            Info = info;
            Capacity = capacity;
        }

        public string Shortname { get; set; }
        public string Info { get; set; }
        public int Capacity { get; set; }
    }
}