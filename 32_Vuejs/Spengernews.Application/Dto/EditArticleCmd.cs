using Microsoft.Extensions.DependencyInjection;
using Spengernews.Application.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Spengernews.Application.Dto
{

    public record EditArticleCmd(
        Guid Guid,
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Die Länge der Überschrift ist ungültig.")]
        string Headline,
        [StringLength(65535, MinimumLength = 3, ErrorMessage = "Die Länge des Contents ist ungültig.")]
        string Content,
        [Url(ErrorMessage = "Ungültige URL.")]
        [StringLength(65535, MinimumLength = 3, ErrorMessage = "Die Länge der URL ist ungültig.")]
        string ImageUrl,
        Guid CategoryGuid);
}
