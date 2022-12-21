
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NewsAggregator.Core.Abstractions;
using NewsAggregator.Core.DataTransferObjects;
using NewsAggregator.Data.Abstractions;
using NewsAggregator.DataBase;
using NewsAggregator.DataBase.Entities;
using Serilog;

namespace NewsAggregator.Business.ServicesImplementations;

public class SourceService : ISourceService
{
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;

    public SourceService(GoodNewsAggregatorContext databaseContext,
        IMapper mapper, 
        IConfiguration configuration, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<SourceDto>> GetSourcesAsync()
    {
        try
        {
            return await _unitOfWork.Sources.Get()
            .Select(source => _mapper.Map<SourceDto>(source))
            .ToListAsync();
        }
        
        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : GetSourcesAsync");
            throw;
        }
    }

    public async Task<SourceDto> GetSourcesByIdAsync(Guid id)
    {
        try
        {
            return _mapper.Map<SourceDto>(await _unitOfWork.Sources.GetByIdAsync(id));
        }
        
        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : GetSourcesByIdAsync");
            throw;
        }
    }

    public async Task<int> CreateSourceAsync(SourceDto dto)
    {
        try
        {
            var entity = _mapper.Map<Source>(dto);
            await _unitOfWork.Sources.AddAsync(entity);
            return await _unitOfWork.Commit();
        }
        
        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : CreateSourcesAsync");
            throw;
        }
    }
}