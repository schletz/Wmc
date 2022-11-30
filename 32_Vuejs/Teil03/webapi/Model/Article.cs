using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Numerics;

namespace webapi.Model
{
    public class Article
    {
        public Article(string headline, string content, DateTime created, string imageUrl, Author author)
        {
            Headline = headline;
            Content = content;
            Created = created;
            ImageUrl = imageUrl;
            Author = author;
        }
#pragma warning disable CS8618 
        protected Article() { }
#pragma warning restore CS8618
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }
        [MaxLength(255)]
        public string Headline { get; set; }
        [MaxLength(4096)]
        public string Content { get; set; }
        public DateTime Created { get; set; }
        [MaxLength(255)]
        public string ImageUrl { get; set; }
        public Author Author { get; set; }
    }


}
