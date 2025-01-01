namespace Chatty.Core.Contracts.Routes;

public static class SenseiApiRoutes
{
    private const string Base = "api/sensei";

    public static class Subscriptions
    {
        private const string SubscriptionsBase = $"{Base}/subscribe";

        public const string UserProfile = $"{SubscriptionsBase}/user-profile";

        public const string Tag = "Subscriptions";
    }
    
    public static class Site
    {
        private const string SubscriptionsBase = $"{Base}/site";

        public const string UserProfile = $"{SubscriptionsBase}/user-session";

        public const string Tag = "Site";
    }
}