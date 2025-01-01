namespace Sensei.Core.Contracts.Routes;

public static class AuthenticationApiRoutes
{
    public static class Authentication
    {
        private const string Base = "api/authentication";
        
        public const string Login = $"{Base}/login";

        public const string Register = $"{Base}/register";
        public const string Refresh = $"{Base}refresh";
        public const string ConfirmEmail = $"{Base}/confirm-email";
        public const string ResendConfirmEmail = $"{Base}/resend-confirm-email";
        public const string ForgotPassword = $"{Base}/forgot-password";
        public const string ResetPassword = $"{Base}/reset-password";
    }
}