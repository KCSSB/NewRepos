namespace API.Exceptions.Enumes
{
    public static class OperationName
    {
        public const string Register = "Register";
        public const string Login = "Login";
        public const string RefreshAccessToken = "RefreshAccessToken";
        public const string Logout = "Logout";

        public const string CreateProject = "CreateProject";

        public const string RegisterAsync = "RegisterAsync";
        public const string HashToken = "HashToken";
        public const string VerifyToken = "VerifyToken";
        public const string GenerateAccessToken = "GenerateAccessToken";
        public const string CreateRefreshTokenAsync = "CreateRefreshTokenAsync";
        public const string RefreshTokenAsync = "RefreshTokenAsync";
        public const string RevokeRefreshTokenAsync = "RevokeRefreshTokenAsync";
        public const string TokenExtractorId = "TokenExtractorId";

        public const string CreateGroupAsync = "CreateGroupAsync";
        public const string AddUserInGroupAsync = "AddUserInGroupAsync";
        public const string CreateProjectAsync = "CreateProjectAsync";
        public const string AddUserInProjectAsync = "AddUserInProjectAsync";

        public const string LoginAsync = "LoginAsync";
    }
}
