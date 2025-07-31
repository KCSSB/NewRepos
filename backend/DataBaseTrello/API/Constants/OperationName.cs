namespace API.Constants
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
        public const string GetUserIdAsGuidOrThrow = "GetUserIdAsGuidOrThrow";

        public const string CreateGroupAsync = "CreateGroupAsync";
        public const string CreateGlobalGroupAsync = "CreateGlobalGroupAsync";
        public const string AddUserInGroupAsync = "AddUserInGroupAsync";
        public const string CreateProjectAsync = "CreateProjectAsync";
        public const string AddUserInProjectAsync = "AddUserInProjectAsync";
        public const string AddFirstUserInGroupAsync = "AddFirstUserInGroupAsync";

        public const string LoginAsync = "LoginAsync";
        public const string UploadUserAvatar = "UploadUserAvatar";
        public const string UploadUserAvatarAsync = "UploadUserAvatarAsync";

    }
}
