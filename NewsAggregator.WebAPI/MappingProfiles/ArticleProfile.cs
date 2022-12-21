using AutoMapper;
using NewsAggregator.Core.DataTransferObjects;
using NewsAggregator.DataBase.Entities;
using NewsAggregator.WebAPI.Models.Requests;

namespace NewsAggregator.WebAPI.MappingProfiles;

public class ArticleProfile : Profile
{
    public ArticleProfile()
    {
        CreateMap<Article, ArticleDto>()
            .ForMember(dto => dto.Id,
                opt =>
                    opt.MapFrom(article => article.Id))
            .ForMember(dto => dto.Text,
                opt
                    => opt.MapFrom(article => article.Text))
            .ForMember(dto => dto.ShortSummary,
                opt
                    => opt.MapFrom(article => article.ShortSummary));

        CreateMap<ArticleDto, Article>()
            .ForMember(dto => dto.Text,
                opt
                    => opt.MapFrom(article => article.Text))
            .ForMember(article => article.ShortSummary, opt
                => opt.MapFrom(article => article.ShortSummary));

        CreateMap<AddArticleRequestModel, ArticleDto>();
        CreateMap<ArticleDto, AddArticleRequestModel>();

        CreateMap<AddOrUpdateArticleRequestModel, ArticleDto>();
        CreateMap<ArticleDto, AddOrUpdateArticleRequestModel>();


    }
}