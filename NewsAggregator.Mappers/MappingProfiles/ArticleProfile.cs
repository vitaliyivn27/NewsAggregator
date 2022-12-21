using NewsAggregator.DataBase.Entities;
using NewsAggregator.Core.DataTransferObjects;
using AutoMapper;


namespace NewsAggregator.Mappers.MappingProfiles;

public class ArticleProfile : Profile
{
    public ArticleProfile()
    {
        CreateMap< Article, ArticleDto>()
            .ForMember(dto => dto.Id,
                opt =>
                    opt.MapFrom(article => article.Id))
            .ForMember(dto => dto.Text,
                opt
                    => opt.MapFrom(article => article.Text))
            .ForMember(dto => dto.ShortSummary,
                opt
                    => opt.MapFrom(article => article.ShortSummary))
            .ForMember(dto => dto.Category,
                opt
                    => opt.MapFrom(article => "Default"));
        
        CreateMap<ArticleDto, Article>()
            .ForMember(dto => dto.Text,
                opt
                    => opt.MapFrom(article => article.Text))
            .ForMember(article => article.ShortSummary, 
                opt
                    => opt.MapFrom(article => article.ShortSummary))
            .ForMember(article => article.SourceId,
                opt
                    => opt.MapFrom(article => article.SourceId));
            //.ForMember(article => article.SourceId, 
            //    opt
            //        => opt.MapFrom(article => new Guid("8F2F5A0A-9FED-4D55-971A-CC909B231389")));
    }
}