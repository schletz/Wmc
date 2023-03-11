using Microsoft.EntityFrameworkCore;
using System;

namespace WmcApi.Model
{
    [Index(nameof(Accountname), IsUnique = true)]
    public class Student : Entity<int>
    {
        public Student(string accountname, string firstname, string lastname, Gender gender, DateTime dateOfBirth, string email, Address address, Schoolclass @class)
        {
            Accountname = accountname;
            Firstname = firstname;
            Lastname = lastname;
            Gender = gender;
            DateOfBirth = dateOfBirth;
            Email = email;
            Address = address;
            Class = @class;
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected Student()
        { }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Accountname { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public Address Address { get; set; }
        public Schoolclass Class { get; set; }
    }
}