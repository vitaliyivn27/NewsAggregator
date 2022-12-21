

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregator.Core.Abstractions;
using NewsAggregatorMvcApp.Models;
using Serilog;

namespace NewsAggregatorMvcApp.Controllers
{
    public class SourceController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ISourceService _sourceService;

        public SourceController(IMapper mapper, ISourceService sourceService)
        {
            _mapper = mapper;
            _sourceService = sourceService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var sources = (await _sourceService.GetSourcesAsync())
                .Select(dto => _mapper.Map<SourceModel>(dto))
                .ToList();

                return View(sources);
            }
            
            catch (Exception exception)
            {
                Log.Error(exception, "ActionName : Index, ControllerName : Source");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateSourceModel model)
        {
            return View();
        }
    }
}
