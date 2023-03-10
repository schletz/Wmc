using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Spengernews.Application.Dto;
using Spengernews.Application.Infrastructure;
using Spengernews.Application.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Spengernews.Webapi.Services
{
    public class ArticleService
    {
        private readonly SpengernewsContext _db;
        private readonly AuthService _authService;
        private readonly IMapper _mapper;

        public ArticleService(SpengernewsContext db, AuthService authService, IMapper mapper)
        {
            _db = db;
            _authService = authService;
            _mapper = mapper;
        }

        public IQueryable<Article> Articles => _db.Articles.AsQueryable();
        public async Task<ArticleDto> AddArticle(NewArticleCmd articleCmd)
        {
            var author = await _db.Authors.FirstOrDefaultAsync(a => a.Guid == articleCmd.AuthorGuid);
            if (author is null)
                throw new ServiceException($"Author {articleCmd.AuthorGuid} does not exist.");
            var category = await _db.Categories.FirstOrDefaultAsync(c => c.Guid == articleCmd.CategoryGuid);
            if (category is null)
                throw new ServiceException($"Category {articleCmd.CategoryGuid} does not exist.");

            var article = _mapper.Map<Article>(articleCmd,
                opt => opt.AfterMap((dto, entity) =>
                {
                    entity.Author = author;
                    entity.Category = category;
                    entity.Created = DateTime.UtcNow;
                    entity.Published = _authService.CurrentUserRole == Role.Admin;
                }));
            _db.Articles.Add(article);
            try { _db.SaveChanges(); }
            catch (DbUpdateException e) { throw new ServiceException(e.InnerException?.Message ?? e.Message, e); } // DB constraint violations, ...
            return _mapper.Map<Article, ArticleDto>(article);
        }
        public async Task<(bool success, string message)> Confirm(Guid articleGuid)
        {
            var article = await _db.Articles.FirstOrDefaultAsync(a => a.Guid == articleGuid);
            if (article is null) { return (false, $"Article {articleGuid} does not exist."); }
            if (_authService.CurrentUserRole != Application.Model.Role.Admin)
                return (false, "User is not an Admin");
            article.Published = true;
            try { await _db.SaveChangesAsync(); }
            catch (DbUpdateException e) { return (false, e.InnerException?.Message ?? e.Message); } // DB constraint violations, ...
            return (true, string.Empty);
        }


        public async Task<(bool success, string message)> EditArticle(EditArticleCmd articleCmd)
        {
            var article = await _db.Articles.FirstOrDefaultAsync(a => a.Guid == articleCmd.Guid);
            if (article is null) { return (false, $"Article {articleCmd.Guid} does not exist."); }
            if (_authService.CurrentUserRole != Application.Model.Role.Admin && article.Published)
                return (false, "Article is published");
            var category = await _db.Categories.FirstOrDefaultAsync(c => c.Guid == articleCmd.CategoryGuid);
            if (category is null) { return (false, $"Category {articleCmd.CategoryGuid} does not exist."); }

            // Overwrite infos in article with new data in articleDto. Don't forget to resolve
            // the foreign key guids!
            _mapper.Map(articleCmd, article,
                opt => opt.AfterMap((dto, entity) =>
                {
                    entity.Category = category;
                }));

            try { await _db.SaveChangesAsync(); }
            catch (DbUpdateException e) { return (false, e.InnerException?.Message ?? e.Message); } // DB constraint violations, ...
            return (true, string.Empty);
        }
        public async Task<(bool success, string message)> Delete(Guid articleGuid)
        {
            var article = await _db.Articles.FirstOrDefaultAsync(a => a.Guid == articleGuid);
            if (article is null) { return (false, $"Article {articleGuid} does not exist."); }
            // Guard clause
            if (_authService.CurrentUserRole != Application.Model.Role.Admin && article.Published)
                return (false, "Article is published");

            _db.Articles.Remove(article);
            try { await _db.SaveChangesAsync(); }
            catch(DbUpdateException e) { return (false, e.InnerException?.Message ?? e.Message); }
            return (true, string.Empty);
        }
    }
}
