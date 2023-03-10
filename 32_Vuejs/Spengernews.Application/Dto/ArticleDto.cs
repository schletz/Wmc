using System;

namespace Spengernews.Application.Dto
{
    public record ArticleDto(
        Guid Guid, string Headline, string Content,
        DateTime Created, string ImageUrl, Guid AuthorGuid, string AuthorFirstname,
        string AuthorLastname, Guid CategoryGuid, string CategoryName);
}
