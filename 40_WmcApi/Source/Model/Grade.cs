using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WmcApi.Model
{
    public class Grade : Entity<int>
    {
        public Grade(Teacher teacher, Student student, string subjectShortname, string subjectLongname, int value, DateTime created)
        {
            Teacher = teacher;
            Student = student;
            SubjectShortname = subjectShortname;
            SubjectLongname = subjectLongname;
            Value = value;
            Created = created;
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected Grade()
        { }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; }

        [ForeignKey("StudentId")]
        public Student Student { get; set; }

        public string SubjectShortname { get; set; }
        public string SubjectLongname { get; set; }
        public int Value { get; set; }
        public DateTime Created { get; set; }
    }
}