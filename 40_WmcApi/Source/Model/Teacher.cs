using Microsoft.EntityFrameworkCore;
using System;

namespace WmcApi.Model
{
    [Index(nameof(Shortname), IsUnique = true)]
    public class Teacher : Entity<int>
    {
        public Teacher(string shortname, string lastname, string firstname, string? title = null)
        {
            Shortname = shortname;
            Lastname = lastname;
            Firstname = firstname;
            Title = title;
        }

        public string Shortname { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string? Title { get; set; }
    }
}