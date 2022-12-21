
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregator.Core.Abstractions;
using NewsAggregator.Core.DataTransferObjects;
using NewsAggregator.WebAPI.Models.Requests;
using NewsAggregator.WebAPI.Models.Responses;
using NewsAggregatorMvcApp.Models;
using Serilog;

namespace NewsAggregator.WebAPI.Controllers
{
    /// <summary>
    /// Controller for work with sources
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SourcesController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly ISourceService _sourceService;
        private readonly IMapper _mapper;


        public SourcesController(IArticleService articleService,
            ISourceService sourceService,
            IMapper mapper)
        {
            _articleService = articleService;
            _sourceService = sourceService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all sources
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSources")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var sources = (await _sourceService.GetSourcesAsync())
                .Select(dto => _mapper.Map<SourceModel>(dto))
                .ToList();

                return Ok(sources);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorModel { Message = ex.Message });
            }
        }

        /// <summary>
        /// Create source
        /// </summary>
        /// <param name="model">Source model</param>
        /// <returns></returns>
        [HttpPut("CreateSource")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSource([FromBody] AddSourceRequestModel? model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Id = Guid.NewGuid();
                    model.SourceType = 0;
                    var dto = _mapper.Map<SourceDto>(model);
                    await _sourceService.CreateSourceAsync(dto);
                }
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorModel { Message = ex.Message });
            }
        }
    }
}