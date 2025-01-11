namespace Chatty.webApi.Contracts;

public static class ApiConstants
{
    private const string BaseRoute = "api";
    public const string JsonContentType = "application/json";
   
    public static class EndpointTags
    {
        public const string Messages = "Messages";
        public const string Groups = "Groups";
    }

    public static class Routes
    {
        public static class Messages
        {
            public const string MessagesBase = $"{BaseRoute}/messages";
        }
        
        public static class Groups
        {
            public const string GroupsBase = $"{BaseRoute}/groups";
            
            public const string AddUsers = $"{GroupsBase}/add-users";
            public const string Leave = $"{GroupsBase}/leave";
            public const string Participants = $"{GroupsBase}/participants/{{groupId:Guid}}";
            public const string Mine = $"{GroupsBase}/mine";
        }
    }
}