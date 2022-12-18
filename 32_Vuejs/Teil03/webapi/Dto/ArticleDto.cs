using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using webapi.Infrastructure;

namespace webapi.Dto
{
    public record ArticleDto(
        Guid Guid,

        [StringLength(255, MinimumLength = 3, ErrorMessage = "Die Länge der Überschrift ist ungültig.")]
        string Headline,

        [StringLength(65535, MinimumLength = 3, ErrorMessage = "Die Länge des Contents ist ungültig.")]
        string Content,

        [Url(ErrorMessage = "Ungültige URL.")]
        [StringLength(65535, MinimumLength = 3, ErrorMessage = "Die Länge der URL ist ungültig.")]
        string ImageUrl,

        Guid AuthorGuid,
        Guid CategoryGuid) : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // We have registered SpengernewsContext in Program.cs in ASP.NET core. So we can
            // get this service to access the database for further validation.
            var db = validationContext.GetRequiredService<SpengernewsContext>();
            if (!db.Authors.Any(a => a.Guid == AuthorGuid))
            {
                yield return new ValidationResult("Author does not exist", new[] { nameof(AuthorGuid) });
            }
            if (!db.Categories.Any(c => c.Guid == CategoryGuid))
            {
                yield return new ValidationResult("Category does not exist", new[] { nameof(CategoryGuid) });
            }
        }
    }
}
