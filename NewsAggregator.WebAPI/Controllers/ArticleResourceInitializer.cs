
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregator.Core.Abstractions;
using NewsAggregator.WebAPI.Models.Responses;

namespace NewsAggregator.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleResourceInitializer : ControllerBase
    {

        private readonly IArticleService _articleService;
        private readonly ISourceService _sourceService;
        private readonly IMapper _mapper;

        public ArticleResourceInitializer(IArticleService articleService, 
            ISourceService sourceService, 
            IMapper mapper)
        {
            _articleService = articleService;
            _sourceService = sourceService;
            _mapper = mapper;
        }

        [Route("AddJobs")]
        [HttpPost]
        public async Task<IActionResult> AddArticlesJobs()
        {
            try
            {
                RecurringJob.RemoveIfExists(nameof(_articleService.AggregateArticlesFromExternalSourcesAsync));

                RecurringJob.AddOrUpdate(() => _articleService.GetAllArticleDataFromAllSourcesRssAsync(),
                    "0 0 * * *");

                RecurringJob.AddOrUpdate(()=>_articleService.AddArticleTextToArticlesAsync(),
                    "5 0 * * *");
                
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel() { Message = ex.Message });
            }
        }

        [Route("AddNews")]
        [HttpGet]                               
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddNews()
        {
            try
            {
                await _articleService.AggregateArticlesFromExternalSourcesAsync();
                return Ok();
            }
            catch(Exception ex)
            {
                return StatusCode(500, new ErrorModel() { Message = ex.Message });
            }
        }

        /*[HttpGet]
        public async Task<IActionResult> RateArticles()
        {
            try
            {
                await _articleService.AddRateToArticlesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorModel() { Message = ex.Message });
            }
        }*/
    }
}
