using AutoMapper;
using Bogus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spengernews.Application.Dto;
using Spengernews.Application.Infrastructure;
using Spengernews.Application.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Webapi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class NewsController : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly SpengernewsContext _db;

        public NewsController(SpengernewsContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        /// <summary>
        /// GET Request /api/news/
        /// Returns a list of all articles with base information.
        /// </summary>
        [HttpGet]
        public IActionResult GetAllNews()
        {
            // Project your entities to a custon JSON WITHOUT INTERNAL KEYS, ...
            var news = _db.Articles.OrderBy(a => a.Created)
                .Select(a => new
                {
                    a.Guid,
                    a.Headline,
                    a.Content,
                    a.Created,
                    a.ImageUrl,
                    AuthorGuid = a.Author.Guid,
                    AuthorFirstname = a.Author.Firstname,
                    AuthorLastname = a.Author.Lastname,
                    CategoryGuid = a.Category.Guid,
                    CategoryName = a.Category.Name
                })
                .ToList();
            return Ok(news);
        }

        /// <summary>
        /// GET Request /api/news/{id}
        /// </summary>
        [HttpGet("{guid:Guid}")]
        public IActionResult GetNewsDetail(Guid guid)
        {
            // Project your entities to a custon JSON WITHOUT INTERNAL KEYS, ...
            var article = _db.Articles
                .Where(a => a.Guid == guid)
                .Select(a => new
                {
                    a.Guid,
                    a.Headline,
                    a.Content,
                    a.Created,
                    a.ImageUrl,
                    AuthorGuid = a.Author.Guid,
                    AuthorFirstname = a.Author.Firstname,
                    AuthorLastname = a.Author.Lastname,
                    CategoryGuid = a.Category.Guid,
                    CategoryName = a.Category.Name
                })
                .FirstOrDefault(a => a.Guid == guid);
            if (article is null) { return NotFound(); }
            return Ok(article);
        }

        /// <summary>
        /// POST Request /api/news with JSON body
        /// Creates a new article in the database. Validation of the dto class is performed
        /// automatically by ASP.NET Core, so you have to implement this in your dto class!
        /// </summary>
        [Authorize]
        [HttpPost]
        public IActionResult AddArticle(ArticleDto articleDto)
        {
            // After mapping we have to resolve the foreign key guids.
            // First() throws an exception if no data matches the predicate. So you have to check
            // the referenced data in your Validate method of your dto class!
            var article = _mapper.Map<Article>(articleDto,
                opt => opt.AfterMap((dto, entity) =>
                {
                    entity.Author = _db.Authors.First(a => a.Guid == articleDto.AuthorGuid);
                    entity.Category = _db.Categories.First(c => c.Guid == articleDto.CategoryGuid);
                    entity.Created = DateTime.UtcNow;
                }));
            _db.Articles.Add(article);
            try { _db.SaveChanges(); }
            catch (DbUpdateException) { return BadRequest(); } // DB constraint violations, ...
            return Ok(_mapper.Map<Article, ArticleDto>(article));
        }

        /// <summary>
        /// PUT Request /api/news/(guid) with JSON body
        /// Updates an article in the database. Validation of the dto class is performed
        /// automatically by ASP.NET Core, so you have to implement this in your dto class!
        /// </summary>
        [Authorize]
        [HttpPut("{guid:Guid}")]
        public IActionResult EditArticle(Guid guid, ArticleDto articleDto)
        {
            if (guid != articleDto.Guid) { return BadRequest(); }
            var article = _db.Articles.FirstOrDefault(a => a.Guid == guid);
            if (article is null) { return NotFound(); }
            // Overwrite infos in article with new data in articleDto. Don't forget to resolve
            // the foreign key guids!
            _mapper.Map(articleDto, article,
                opt => opt.AfterMap((dto, entity) =>
                {
                    entity.Author = _db.Authors.First(a => a.Guid == articleDto.AuthorGuid);
                    entity.Category = _db.Categories.First(c => c.Guid == articleDto.CategoryGuid);
                }));

            try { _db.SaveChanges(); }
            catch (DbUpdateException) { return BadRequest(); } // DB constraint violations, ...
            return NoContent();
        }

        /// <summary>
        /// DELETE Request /api/news/(guid) with JSON body
        /// Updates an article in the database.
        /// </summary>
        [Authorize]
        [HttpDelete("{guid:Guid}")]
        public IActionResult DeleteArticle(Guid guid)
        {
            // Try to find article in the database.
            var article = _db.Articles.FirstOrDefault(a => a.Guid == guid);
            // Article does not exist: return 404.
            if (article is null) { return NotFound(); }
            // TODO: Remove referenced data (if needed)
            // Remove article.
            _db.Articles.Remove(article);
            try { _db.SaveChanges(); }
            catch (DbUpdateException) { return BadRequest(); } // DB constraint violations, ...
            return NoContent();
        }
    }
}
