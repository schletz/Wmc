using static System.Formats.Asn1.AsnWriter;
using System;
using webapi.Model;
using AutoMapper;

namespace webapi.Dto
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
