using AutoMapper;
using Spengernews.Application.Model;

namespace Spengernews.Application.Dto

{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<NewArticleCmd, Article>();  // NewArticleCmd --> Article
            CreateMap<EditArticleCmd, Article>();  // EditArticleCmd --> Article

            CreateMap<Article, ArticleDto>();  // Article --> ArticleDto
        }
    }
}
