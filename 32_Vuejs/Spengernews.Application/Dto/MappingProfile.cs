using AutoMapper;
using Spengernews.Application.Model;

namespace Spengernews.Application.Dto

{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ArticleDto, Article>();  // ArticleDto --> Article
            CreateMap<Article, ArticleDto>();  // Article --> ArticleDto
        }
    }
}
