namespace Api.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SwaggerApiKeyAttribute : Attribute { }
}
