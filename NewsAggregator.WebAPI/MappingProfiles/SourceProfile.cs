using AutoMapper;
using NewsAggregator.Core.DataTransferObjects;
using NewsAggregator.DataBase.Entities;
using NewsAggregator.WebAPI.Models.Requests;

namespace NewsAggregator.WebAPI.MappingProfiles;

public class SourceProfile : Profile
{
    public SourceProfile()
    {
        CreateMap<Source, SourceDto>();
        CreateMap<SourceDto, Source>();

        CreateMap<AddSourceRequestModel, SourceDto>();
        CreateMap<SourceDto, AddSourceRequestModel>();


    }
}