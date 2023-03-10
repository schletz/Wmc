using System;
using System.ComponentModel.DataAnnotations;

namespace Spengernews.Application.Dto
{
    public record NewArticleCmd(
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Die Länge der Überschrift ist ungültig.")]
        string Headline,
        [StringLength(65535, MinimumLength = 3, ErrorMessage = "Die Länge des Contents ist ungültig.")]
        string Content,
        [Url(ErrorMessage = "Ungültige URL.")]
        [StringLength(65535, MinimumLength = 3, ErrorMessage = "Die Länge der URL ist ungültig.")]
        string ImageUrl,
        Guid AuthorGuid,
        Guid CategoryGuid);
}
