using static System.Formats.Asn1.AsnWriter;
using System;
using Webapi.Model;
using AutoMapper;

namespace Webapi.Dto
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
