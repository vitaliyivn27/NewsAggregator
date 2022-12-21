using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregator.Core.Abstractions;
using NewsAggregator.Core.DataTransferObjects;
using NewsAggregator.DataBase.Entities;
using NewsAggregator.WebAPI.Models.Requests;
using NewsAggregator.WebAPI.Models.Responses;

namespace NewsAggregator.WebAPI.Controllers
{
    /// <summary>
    /// Controller for work with articles
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly IMapper _mapper;
        

        public ArticlesController(IArticleService articleService,
            ISourceService sourceService,
            IMapper mapper)
        {
            _articleService = articleService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get article from storage with specified id
        /// </summary>
        /// <param name="id">Id of article</param>
        /// <returns></returns>
        [HttpGet("GetArticleById")]
        [ProducesResponseType(typeof(ArticleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Nullable), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetArticleById(Guid id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            return Ok(article);
        }

        /// <summary>
        /// Get articles by source id page size and page number
        /// </summary>
        /// <param name="model">Contains source id page size and page number</param>
        /// <returns></returns>
        [Route("GetArticlesBySourceId")]
        [HttpGet]
        public async Task<IActionResult> GetArticles([FromQuery] GetArticlesRequestModel? model)
        {
            if (model!= null)
            {
                if (model.PageSize == 0)
                {
                    model.PageSize = 5;
                }
                var articles = await _articleService
                    .GetArticlesBySourceIdAsync(model?.SourceId, model.PageSize, model.PageNumber);

                return Ok(articles.ToList());
            }
            return BadRequest();
        }

        /// <summary>
        /// Create article
        /// </summary>
        /// <param name="model">Contains article model</param>
        /// <returns></returns>
        [HttpPut("CreateArticle")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateArticle([FromBody] AddArticleRequestModel? model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Id = Guid.NewGuid();
                    model.PublicationDate = DateTime.Now;
                    var dto = _mapper.Map<ArticleDto>(model);
                    await _articleService.CreateArticleAsync(dto);                   
                }
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorModel { Message = ex.Message });
            }
        }

        /// <summary>
        /// Update article
        /// </summary>
        /// <param name="model">Contains article id and patch model</param>
        /// <returns></returns>
        [HttpPatch("UpdateArticleById")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateArticle(Guid id, [FromBody] AddOrUpdateArticleRequestModel? model)
        {           
            try
            {
                if (model != null)
                {
                    var dto = _mapper.Map<ArticleDto>(model);

                    await _articleService.UpdateArticleAsync(id, dto);
                }
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorModel { Message = ex.Message });
            }
        }

        /// <summary>
        /// Delete Article
        /// </summary>
        /// <param name="id">Id of article</param>
        /// <returns></returns>
        [HttpDelete("DeleteArticleById")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Nullable), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteArticle(Guid id)
        {
            try
            {
                await _articleService.DeleteArticleAsync(id);

                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorModel { Message = ex.Message });
            }
        }
    }
}
