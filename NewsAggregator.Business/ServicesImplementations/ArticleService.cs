using AutoMapper;
using HtmlAgilityPack;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NewsAggregator.Business.Models;
using NewsAggregator.Core;
using NewsAggregator.Core.Abstractions;
using NewsAggregator.Core.DataTransferObjects;
using NewsAggregator.Data.Abstractions;
using NewsAggregator.Data.CQS.Commands;
using NewsAggregator.Data.CQS.Queries;
using NewsAggregator.DataBase.Entities;
using Newtonsoft.Json;
using Serilog;
using System.Net.Http.Json;
using System.ServiceModel.Syndication;
using System.Xml;
using X.PagedList;

namespace NewsAggregator.Business.ServicesImplementations;

public class ArticleService : IArticleService
{
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;


    public ArticleService(IMapper mapper,
        IConfiguration configuration,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _mapper = mapper;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }


    public async Task<List<ArticleDto>> GetArticlesByPageNumberAndPageSizeAsync(int pageNumber,
        int pageSize)
    {
        try
        {
            var myApiKey = _configuration.GetSection("Secrets")["MyApiKey"];
            //_configuration.
            var list = await _unitOfWork.Articles
                .Get()
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .Select(article => _mapper.Map<ArticleDto>(article))
                .ToListAsync();
            return list;
        }

        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : GetArticlesByPageNumberAndPageSizeAsync");
            throw;
        }
    }

    public async Task AggregateArticlesFromExternalSourcesAsync()
    {
        try
        {
            await GetAllArticleDataFromAllSourcesRssAsync();
            await AddArticleTextToArticlesAsync();
            //await AddRateToArticlesAsync();
        }

        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : AggregateArticlesFromExternalSourcesAsync");
            throw;
        }
    }

    public async Task<IEnumerable<ArticleDto>> GetArticlesBySourceIdAsync(Guid? sourceId,
        int pageSize = 5,
        int pageNumber = 0)
    {
        try
        {           
            var list = new List<ArticleDto>();

            var entities = _unitOfWork.Articles.Get();

            if (sourceId != null && !Guid.Empty.Equals(sourceId))
            {
                entities = entities.Where(dto => dto.SourceId.Equals(sourceId));
            }

            var smth = entities.ToList();

            var result = (await smth
                   .Skip(pageSize * pageNumber)
                   .Take(pageSize)
                   .ToListAsync())
               .Select(ent => _mapper.Map<ArticleDto>(ent))
               .ToArray();
            
            return result;
        }

        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : GetArticlesByNameAndSourcesAsync");
            throw;
        }
    }

    public async Task<int> GetNumberAsync(Guid? sourceId)
    {
        var query = new GetArticlesNumberQuery
        {
            SourceId = sourceId
        };
        return await _mediator.Send(query);
    }

    public async Task<ArticleDto> GetArticleByIdAsync(Guid id)
    {
        try
        {
            //var entity = await _unitOfWork.Articles.GetByIdAsync(id);         UOW
            //var dto = _mapper.Map<ArticleDto>(entity);
            var dto = _mapper.Map<ArticleDto>(await _mediator.Send(new GetArticleByIdQuery() { Id = id }));

            return dto;
        }

        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : GetArticleByIdAsync");
            throw;
        }
    }

    public async Task<int> CreateArticleAsync(ArticleDto dto)
    {
        var entity = _mapper.Map<Article>(dto);

        if (entity != null)
        {
            await _unitOfWork.Articles.AddAsync(entity);
            var addingResult = await _unitOfWork.Commit();
            return addingResult;
        }
        else
        {
            throw new ArgumentException(nameof(dto));
        }
    }

    public async Task<int> UpdateArticleAsync(Guid id, ArticleDto? dto)
    {
        try
        {
            var sourceDto = await GetArticleByIdAsync(id);
            var patchList = new List<PatchModel>();
            if (dto != null)
            {
                if (!dto.Title.Equals(sourceDto.Title))
                {
                    patchList.Add(new PatchModel()
                    {
                        PropertyName = nameof(dto.Title),
                        PropertyValue = dto.Title
                    });
                }
            }

            await _unitOfWork.Articles.PatchAsync(id, patchList);
            return await _unitOfWork.Commit();
        }

        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : UpdateArticleAsync");
            throw;
        }
    }

    public async Task GetAllArticleDataFromAllSourcesRssAsync()
    {
        try
        {
            var sources = await _unitOfWork.Sources.GetAllAsync();

            foreach (var source in sources)
            {
                await GetAllArticleDataFromOneSoureRssAsync(source.Id, source.RssUrl);
            }
        }

        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : GetAllArticleDataFromAllSourcesRssAsync");
            throw;
        }
    }

    public async Task AddArticleTextToArticlesAsync()
    {
        try
        {
            //var articlesWithEmptyTextIds = _unitOfWork.Articles.Get()
            //    .Where(article => string.IsNullOrEmpty(article.Text))       UOW
            //    .Select(article => article.Id)
            //    .ToList();
            var articlesWithEmptyTextIds = await _mediator
                .Send(new GetAllArticlesWithoutTextIdsQuery());

            if (articlesWithEmptyTextIds != null)
            {
                foreach (var articleId in articlesWithEmptyTextIds)
                {
                    await AddArticleTextToArticleAsync(articleId);
                }
            }                
        }

        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : AddArticleTextToArticlesAsync");
            throw;
        }
    }

    /*public async Task AddRateToArticlesAsync()
    {
        try
        {
            var articlesWithEmptyRateIds = _unitOfWork.Articles.Get()
                .Where(article => article.Rate == null && !string.IsNullOrEmpty(article.Text))
                .Select(article => article.Id)
                .ToList();

            foreach (var articleId in articlesWithEmptyRateIds)
            {
                await RateArticleAsync(articleId);
            }
        }

        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : AddRateToArticlesAsync");
            throw;
        }
    }*/

    public async Task DeleteArticleAsync(Guid id)
    {
        try
        {
            if (id != Guid.Empty)
            {
                var entity = await _unitOfWork.Articles.GetByIdAsync(id);

                if (entity != null)
                {
                    _unitOfWork.Articles.Remove(entity);

                    await _unitOfWork.Commit();
                }
            }
            else
            {
                throw new ArgumentException("Article for removing doesn't exist", nameof(id));
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : DeleteArticleAsync");
            throw;
        }
    }

    private async Task GetAllArticleDataFromOneSoureRssAsync(Guid sourceId, string? sourceRssUrl)
    {
        try
        {
            var list = new List<ArticleDto>();

            if (!string.IsNullOrEmpty(sourceRssUrl) && sourceId.ToString("D").ToUpper().Equals("8F2F5A0A-9FED-4D55-971A-CC909B231389")) // Onliner.RSS
            {
                using (var reader = XmlReader.Create(sourceRssUrl))
                {
                    var feed = SyndicationFeed.Load(reader);

                    foreach (var item in feed.Items)
                    {
                        var articleDto = new ArticleDto()
                        {
                            Id = Guid.NewGuid(),
                            Title = item.Title.Text,
                            PublicationDate = item.PublishDate.UtcDateTime,
                            ShortSummary = item.Summary.Text.Replace("Читать далее…", " "),
                            Category = item.Categories.FirstOrDefault()?.Name,
                            SourceId = sourceId,
                            SourceUrl = item.Id
                        };
                        list.Add(articleDto);
                    }
                }
            }
            if (!string.IsNullOrEmpty(sourceRssUrl) && sourceId.ToString("D").ToUpper().Equals("BBA4C71A-E3E6-4303-A75C-45FB50E3E58E")) // DevBy.RSS
            {
                using (var reader = XmlReader.Create(sourceRssUrl))
                {
                    var feed = SyndicationFeed.Load(reader);

                    foreach (var item in feed.Items)
                    {
                        var articleDto = new ArticleDto()
                        {
                            Id = Guid.NewGuid(),
                            Title = item.Title.Text,
                            PublicationDate = item.PublishDate.UtcDateTime,
                            ShortSummary = item.Summary?.Text,
                            Category = item.Categories?.FirstOrDefault()?.Name,
                            SourceId = sourceId,
                            SourceUrl = item.Id
                        };
                        list.Add(articleDto);
                    }
                }
            }

            await _mediator.Send(new AddArticleDataFromRssFeedCommand()
            { Articles = list });
            //var oldArticleUrls = await _unitOfWork.Articles.Get()
            //    .Select(article => article.SourceUrl)
            //    .Distinct()
            //.ToArrayAsync();

            //var entities = list.Where(dto => !oldArticleUrls.Contains(dto.SourceUrl))         UOW
            //    .Select(dto => _mapper.Map<Article>(dto)).ToArray();

            //await _unitOfWork.Articles.AddRangeAsync(entities);
            //await _unitOfWork.Commit();
        }

        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : GetAllArticleDataFromOneSoureRssAsync");
            throw;
        }
    }

    private async Task AddArticleTextToArticleAsync(Guid articleId)
    {
        try
        {
            //var article = await _unitOfWork.Articles.GetByIdAsync(articleId);     UOW
            var article = await _mediator.Send(new GetArticleByIdQuery { Id = articleId });
            if (article == null)
            {
                throw new ArgumentException($"Article with id: {articleId} doesn't exists",
                    nameof(articleId));
            }
            if (article.SourceId.ToString("D").ToUpper().Equals("8F2F5A0A-9FED-4D55-971A-CC909B231389"))  // Onliner.RSS
            {
                var articleSourceUrl = article.SourceUrl;

                var web = new HtmlWeb();
                var htmlDoc = web.Load(articleSourceUrl);
                var nodes =
                    htmlDoc.DocumentNode.Descendants(0)
                        .Where(n => n.HasClass("news-text"));

                if (nodes.Any())
                {
                    var articleText = nodes.FirstOrDefault()?
                    .ChildNodes
                        .Where(node => (node.Name.Equals("p") || node.Name.Equals("div") || node.Name.Equals("h2"))
                                       && !node.HasClass("news-reference")
                                       && !node.HasClass("news-banner")
                                       && !node.HasClass("news-widget")
                                       && !node.HasClass("news-vote")
                                       && !node.HasClass("news-incut")
                                       && !node.HasClass("news-header__title")
                                       && !node.HasClass("news-header__button")             
                                       && node.Attributes["style"] == null)
                        .Select(node => node.OuterHtml)
                        .Aggregate((i, j) => i + Environment.NewLine + j);

                    await _mediator.Send(new UpdateArticleTextByIdCommand() { Id = articleId, Text = articleText });
                    //await _unitOfWork.Articles.UpdateArticleTextAsync(articleId, articleText);        UOW
                    //await _unitOfWork.Commit();
                }
            }
            /*else if (article.SourceId.ToString("D").ToUpper().Equals("C34CF358-9448-404E-B79B-65A44FD4F867")) // Shazoo.RSS
            {
                var articleSourceUrl = article.SourceUrl;

                var web = new HtmlWeb();
                var htmlDoc = web.Load(articleSourceUrl);
                var nodes =
                    htmlDoc.DocumentNode.Descendants(0)
                        .Where(n => n.Name.Equals("article"));

                if (nodes.Any())
                {
                    var articleText = nodes.FirstOrDefault()?
                        .ChildNodes
                        .Where(node => (node.Name.Equals("section") || node.Name.Equals("div"))                                      
                                       && !node.HasClass("hidden")
                                       && !node.HasClass("items-center")
                                       && !node.HasClass("justify-between")
                                       && !node.HasClass("my-4")
                                       && !node.HasClass("relative")
                                       && !node.Name.Equals("aside")
                                       && !node.HasClass("aside")
                                       && !node.HasClass("vue-target")
                                       && node.Attributes["style"] == null)
                        .Select(node => node.OuterHtml)
                        .Aggregate((i, j) => i + Environment.NewLine + j);


                    await _unitOfWork.Articles.UpdateArticleTextAsync(articleId, articleText);
                    await _unitOfWork.Commit();
                }
            }*/
            else if (article.SourceId.ToString("D").ToUpper().Equals("BBA4C71A-E3E6-4303-A75C-45FB50E3E58E"))  // DevBy.RSS
            {
                var articleSourceUrl = article.SourceUrl;

                var web = new HtmlWeb();
                var htmlDoc = web.Load(articleSourceUrl);
                var nodes =
                    htmlDoc.DocumentNode.Descendants(0)
                        .Where(n => n.HasClass("article__body"));

                if (nodes.Any())
                {
                    var articleText = nodes.FirstOrDefault()?
                    .ChildNodes
                        .Where(node => (node.Name.Equals("p") || node.Name.Equals("div") || node.Name.Equals("figure"))
                                       && !node.HasClass("card")
                                       && !node.HasClass("incut")
                                       && !node.HasClass("global-incut")
                                       && !node.HasClass("article-widget")
                                       && node.Attributes["style"] == null)
                        .Select(node => node.OuterHtml)
                        .Aggregate((i, j) => i + Environment.NewLine + j);

                    await _mediator.Send(new UpdateArticleTextByIdCommand() { Id = articleId, Text = articleText });
                    //await _unitOfWork.Articles.UpdateArticleTextAsync(articleId, articleText);        UOW
                    //await _unitOfWork.Commit();           
                }
            }
        }

        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : AddArticleTextToArticleAsync");
            throw;
        }
    }

    /*private async Task RateArticleAsync(Guid articleId)
    {
        try
        {
            var article = await _unitOfWork.Articles.GetByIdAsync(articleId);

            if (article == null)
            {
                throw new ArgumentException($"Article with id: {articleId} doesn't exists",
                    nameof(articleId));
            }

            using (var client = new HttpClient())
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Post,
                    new Uri(@"https://api.ispras.ru/texterra/v1/nlp?targetType=lemma&apikey=36d3f07a81d9a648f0684e21951dd4f49c7df111"));
                httpRequest.Headers.Add("Accept", "application/json");

                httpRequest.Content = JsonContent.Create(new[] { new TextRequestModel() { Text = article.Text } });

                var response = await client.SendAsync(httpRequest);
                var responseStr = await response.Content.ReadAsStreamAsync();

                using (var sr = new StreamReader(responseStr))
                {
                    var data = await sr.ReadToEndAsync();

                    var resp = JsonConvert.DeserializeObject<IsprassResponseObject[]>(data);
                }
            }
        }

        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : RateArticleAsync");
            throw;
        }
    }*/

}