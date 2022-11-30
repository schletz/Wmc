using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Model
{
    [Index(nameof(Email), IsUnique = true)]
    public class Author
    {
        public Author(string firstname, string lastname, string email, Category category, string? phone = null)
        {
            Firstname = firstname;
            Lastname = lastname;
            Email = email;
            Phone = phone;
            Category = category;
        }

#pragma warning disable CS8618
        protected Author() { }
#pragma warning restore CS8618
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public Category Category { get; set; }
    }


}
