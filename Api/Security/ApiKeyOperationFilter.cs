using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Security
{
    public class ApiKeyOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasApiKey = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<SwaggerApiKeyAttribute>()
                .Any()
                || context.MethodInfo.DeclaringType?
                    .GetCustomAttributes(true)
                    .OfType<SwaggerApiKeyAttribute>()
                    .Any() == true;

            if (!hasApiKey)
                return;

            operation.Security ??= new List<OpenApiSecurityRequirement>();

            operation.Security.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    }
                },
                Array.Empty<string>()
            }
        });
        }
    }

}
