using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace WmcApi.Model
{
    [Index(nameof(Shortname), IsUnique = true)]
    public class Schoolclass : Entity<int>
    {
        public Schoolclass(
            string shortname, string department, Teacher classTeacher, int term,
            bool winter, bool summer, Room? room = null)
        {
            Shortname = shortname;
            Department = department;
            ClassTeacher = classTeacher;
            Term = term;
            Winter = winter;
            Summer = summer;
            Room = room;
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected Schoolclass()
        { }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public string Shortname { get; set; }
        public string Department { get; set; }
        public Teacher ClassTeacher { get; set; }
        public int Term { get; set; }
        public bool Winter { get; set; }
        public bool Summer { get; set; }
        public Room? Room { get; set; }
        public List<Student> Students { get; } = new();
        public List<Lesson> Lessons { get; } = new();
    }
}