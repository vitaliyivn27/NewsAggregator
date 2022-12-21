

using Microsoft.AspNetCore.Mvc.Filters;

namespace NewsAggregatorMvcApp.Filters;

public class AddDataToResponseHeaderFilter : Attribute, IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        context.HttpContext.Response.Headers
            .Add("SomeVerySecretKey", Guid.NewGuid().ToString("N"));
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
    }
}