

using AutoMapper;
using NewsAggregator.Core.DataTransferObjects;
using NewsAggregator.DataBase.Entities;
using NewsAggregatorMvcApp.Models;

namespace NewsAggregatorMvcApp.MappingProfiles;

public class ArticleProfile : Profile
{
    public ArticleProfile()
    {
        CreateMap<Article, ArticleDto>();
        CreateMap<ArticleDto, Article>();

            

        CreateMap<ArticleDto, ArticleModel>().ReverseMap();

    }
}