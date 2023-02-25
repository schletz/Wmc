using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spengernews.Application.Model
{
    public class Article
    {
        public Article(
            string headline, string content, DateTime created,
            string imageUrl, bool published, Author author, Category category)
        {
            Headline = headline;
            Content = content;
            Created = created;
            ImageUrl = imageUrl;
            Published = published;
            Author = author;
            Category = category;
        }

#pragma warning disable CS8618

        protected Article()
        { }

#pragma warning restore CS8618

        // Convention over configuration
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }

        public Guid Guid { get; set; }

        [MaxLength(255)]
        public string Headline { get; set; }

        [MaxLength(65535)]
        public string Content { get; set; }

        public DateTime Created { get; set; }

        [MaxLength(255)]
        public string ImageUrl { get; set; }

        public Author Author { get; set; }
        public bool Published { get; set; }
        public Category Category { get; set; }
    }
}