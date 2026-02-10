using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Security
{
    public class ApiKeyFilter : IAsyncActionFilter
    {
        private readonly IConfiguration _config;

        public ApiKeyFilter(IConfiguration config)
        {
            _config = config;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {

            var endpoint = context.HttpContext.GetEndpoint();

            var requiresApiKey =
                endpoint?.Metadata.GetMetadata<ApiKeyAttribute>() != null;

            if (!requiresApiKey)
            {
                await next();
                return;
            }


            var headerName = _config["ApiKey:HeaderName"];
            var expectedKey = _config["ApiKey:Value"];

            if (!context.HttpContext.Request.Headers.TryGetValue(headerName, out var receivedKey))
            {
                context.Result = new UnauthorizedObjectResult("API Key ausente");
                return;
            }

            if (receivedKey != expectedKey)
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }

}
