using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Model
{
    [Index(nameof(Email), IsUnique = true)]
    public class Author
    {
        public Author(string firstname, string lastname, string email, string? phone = null)
        {
            Firstname = firstname;
            Lastname = lastname;
            Email = email;
            Phone = phone;
        }

#pragma warning disable CS8618
        protected Author() { }
#pragma warning restore CS8618
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }
        public Guid Guid { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
    }


}
