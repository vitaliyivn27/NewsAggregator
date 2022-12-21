using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsAggregator.Core.Abstractions;
using NewsAggregator.Core.DataTransferObjects;
using NewsAggregatorMvcApp.Models;
using Serilog;
using X.PagedList.Mvc.Core;
using X.PagedList;
using System;
using System.Drawing.Printing;
using static System.Reflection.Metadata.BlobBuilder;
using NewsAggregator.Data.Abstractions;

namespace NewsAggregatorMvcApp.Controllers
{
    //[Authorize(Roles = "User, Admin")]
    public class ArticleController : Controller
    {

        private readonly IArticleService _articleService;
        private readonly ISourceService _sourceService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private int _pageSize = 5;


        public ArticleController(IArticleService articleService,
            IMapper mapper, ISourceService sourceService,
            IUnitOfWork unitOfWork)
        {
            _articleService = articleService;
            _mapper = mapper;
            _sourceService = sourceService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> GetArticlesAsync(int? page)
        {
            try
            {
                var articles = _unitOfWork.Articles.Get().Select(article => _mapper.Map<ArticleDto>(article));
                var pageNumber = page ?? 1;
                var onePageOfArticles = articles.ToPagedList(pageNumber, 5);

                if (onePageOfArticles.Any())
                {
                    ViewBag.onePageOfArticles = onePageOfArticles;
                    return View();
                }
                else
                {
                    throw new ArgumentException(nameof(page));
                }
            }

            catch (Exception exception)
            {
                Log.Error(exception, "ActionName : IndexAsync, ControllerName : Article");
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var dto = await _articleService.GetArticleByIdAsync(id);
                if (dto != null)
                {
                    return View(dto);
                }
                else
                {
                    return NotFound();
                }
            }

            catch (Exception exception)
            {
                Log.Error(exception, "ActionName : Details, ControllerName : Article");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            //var model = new CreateArticleModel();

            //var sources = await _sourceService.GetSourcesAsync();

            //model.Sources = sources
            //    .Select(dto => new SelectListItem(
            //        dto.Name,
            //        dto.Id.ToString("G")))
            //    .ToList();

            //return View(model);

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ArticleModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Id = Guid.NewGuid();
                    model.PublicationDate = DateTime.Now;

                    var dto = _mapper.Map<ArticleDto>(model);

                    await _articleService.CreateArticleAsync(dto);
                    return RedirectToAction("GetArticles", "Article");
                }

                else
                {
                    return View(model);
                }
            }

            catch (Exception exception)
            {
                Log.Error(exception, "ActionName : Create, ControllerName : Article");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                if (id != Guid.Empty)
                {
                    var articleDto = await _articleService.GetArticleByIdAsync(id);
                    if (articleDto == null)
                    {
                        return BadRequest();
                    }

                    var editModel = _mapper.Map<ArticleModel>(articleDto);

                    return View(editModel);
                }

                return BadRequest();
            }

            catch (Exception exception)
            {
                Log.Error(exception, "ActionName : Edit, ControllerName : Article");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(ArticleModel model)
        {
            try
            {
                if (model != null)
                {
                    var dto = _mapper.Map<ArticleDto>(model);

                    await _articleService.UpdateArticleAsync(model.Id, dto);

                    //await _articleService.CreateArticleAsync(dto);

                    return RedirectToAction("Index", "Article");
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, "ActionName : Edit, ControllerName : Article");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (id != Guid.Empty)
                {
                    var articleDto = await _articleService.GetArticleByIdAsync(id);
                    if (articleDto == null)
                    {
                        return NotFound();
                    }

                    var deleteModel = _mapper.Map<ArticleModel>(articleDto);

                    return View(deleteModel);
                }

                return BadRequest();
            }

            catch (Exception exception)
            {
                Log.Error(exception, "ActionName : Edit, ControllerName : Article");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(ArticleModel model)
        {
            try
            {
                if (model != null)
                {
                    var dto = _mapper.Map<ArticleDto>(model);

                    await _articleService.DeleteArticleAsync(model.Id);

                    return RedirectToAction("Index", "Article");
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, "ActionName : Delete, ControllerName : Article");
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
