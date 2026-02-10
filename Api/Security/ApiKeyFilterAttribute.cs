using Microsoft.AspNetCore.Mvc;

namespace Api.Security
{
    public class ApiKeyFilterAttribute : TypeFilterAttribute
    {
        public ApiKeyFilterAttribute() : base(typeof(ApiKeyFilter))
        {
        }
    }

}
