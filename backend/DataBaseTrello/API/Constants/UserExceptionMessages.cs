namespace API.Constants
{
    public static class UserExceptionMessages
    {
        public const string UnauthorizedExceptionMessage = "Ошибка авторизации";
        public const string InternalExceptionMessage = "Произошла внутренняя ошибка в момент выполнения операции";
        public const string ForbiddenExceptionMessage = "Доступ запрещён";
        public const string BadRequestExceptionMessage = "Введённые вами данные некорректны";
        public const string ConflictExceptionMessage = "Это действие невозможно, так как оно конфликтует с текущими данными";
    }
}
