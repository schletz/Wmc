using AutoMapper;
using Bogus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spengernews.Application.Dto;
using Spengernews.Application.Infrastructure;
using Spengernews.Application.Model;
using Spengernews.Webapi.Services;
using SQLitePCL;
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
        private readonly ArticleService _articleService;

        public NewsController(ArticleService articleService, IMapper mapper)
        {
            _articleService = articleService;
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
            var news = _mapper.ProjectTo<ArticleDto>(
                _articleService.Articles.OrderBy(a => a.Created).Where(a => a.Published));
            return Ok(news);
        }

        /// <summary>
        /// GET Request /api/news/{id}
        /// </summary>
        [HttpGet("{guid:Guid}")]
        public IActionResult GetNewsDetail(Guid guid)
        {
            // Project your entities to a custon JSON WITHOUT INTERNAL KEYS, ...
            var article = _mapper.ProjectTo<ArticleDto>(
                _articleService.Articles.Where(a => a.Guid == guid))
                .FirstOrDefault();
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
        public async Task<IActionResult> AddArticle(NewArticleCmd articleCmd)
        {
            try
            {
                var article = await _articleService.AddArticle(articleCmd);
                return Ok(article);
            }
            catch (ServiceException e) { return BadRequest(e.Message); }
        }

        /// <summary>
        /// PUT Request /api/news/(guid) with JSON body
        /// Updates an article in the database. Validation of the dto class is performed
        /// automatically by ASP.NET Core, so you have to implement this in your dto class!
        /// </summary>
        [Authorize]
        [HttpPut("{guid:Guid}")]
        public async Task<IActionResult> EditArticle(Guid guid, EditArticleCmd articleCmd)
        {
            if (guid != articleCmd.Guid) { return BadRequest(); }
            var (success, message) = await _articleService.EditArticle(articleCmd);
            if (!success) { return BadRequest(message); }
            return NoContent();
        }

        /// <summary>
        /// DELETE Request /api/news/(guid) with JSON body
        /// Updates an article in the database.
        /// </summary>
        [Authorize]
        [HttpDelete("{guid:Guid}")]
        public async Task<IActionResult> DeleteArticle(Guid guid)
        {
            var (success, message) = await _articleService.Delete(guid);
            if (!success) { return BadRequest(message); }
            return NoContent();
        }
    }

}