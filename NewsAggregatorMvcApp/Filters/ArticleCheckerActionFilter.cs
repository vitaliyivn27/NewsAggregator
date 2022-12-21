

using Microsoft.AspNetCore.Mvc.Filters;
using NewsAggregator.Data.Abstractions;
using Serilog;

namespace NewsAggregatorMvcApp.Filters;

public class ArticleCheckerActionFilter : Attribute, IActionFilter
{
    private readonly IUnitOfWork _unitOfWork;
    public ArticleCheckerActionFilter(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        try
        {
            var id = (Guid)context.ActionArguments["id"];
            if (id.Equals(Guid.Empty))
            {
                var randomid = _unitOfWork.Articles.Get().FirstOrDefault().Id;
                context.ActionArguments["id"] = randomid;
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : OnActionExecuting");
            throw;
        }

    }

    public void OnActionExecuted(ActionExecutedContext context)
    {


    }
}