namespace Chatty.webApi.Contracts;

public static class ApiConstants
{
    private const string BaseRoute = "api";
    public const string JsonContentType = "application/json";
   
    public static class EndpointTags
    {
        public const string Messages = "Messages";
    }

    public static class Routes
    {
        public static class Messages
        {
            public const string MessagesBase = $"{BaseRoute}/messages";
        }
    }
}