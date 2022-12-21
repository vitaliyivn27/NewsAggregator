
using Microsoft.AspNetCore.Mvc;
using NewsAggregator.Core.Abstractions;
using NewsAggregator.WebAPI.Models.Requests;

namespace NewsAggregator.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesCountController : ControllerBase
    {

        private readonly IArticleService _articleService;

        public ArticlesCountController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [Route("GetArticlesBySourceIdCount")]
        [HttpGet]
        public async Task<IActionResult> GetArticlesCount([FromQuery] GetArticlesCountRequestModel? model)
        {
            if (model != null)
            {
                var articlesNumber = await _articleService
                    .GetNumberAsync(model.SourceId);

                return Ok(articlesNumber);
            }
            return BadRequest();
        }
    }
}
