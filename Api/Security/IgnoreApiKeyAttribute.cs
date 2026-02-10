namespace Api.Security
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class IgnoreApiKeyAttribute : Attribute
    {
    }
}
