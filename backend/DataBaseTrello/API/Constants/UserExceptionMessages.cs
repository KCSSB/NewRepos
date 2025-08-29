namespace API.Constants
{
    public static class UserExceptionMessages
    {
        public const string CreateProjectExceptionMessage = "Ошибка при создании проекта, повторите попытку позже";
        public const string CreateGroupExceptionMessage = "Ошибка во время создания группы";
       

        public const string RegistrationExceptionMessage = "Ошибка регистрации";
        

        public const string IncorrectDataExceptionMessage = "Вы ввели некорректные данные";
       
        public const string UploadFilesExceptionMessage = "Произошла ошибка при перемещении файлов";

        public const string UnauthorizedExceptionMessage = "Ошибка авторизации";
        public const string InternalExceptionMessage = "Произошла внутренняя ошибка в момент выполнения операции";
        public const string ForbiddenExceptionMessage = "Доступ запрещён";
        public const string BadRequestExceptionMessage = "Введённые вами данные некорректны";
        public const string ConflictExceptionMessage = "Это действие невозможно, так как оно конфликтует с текущими данными";
    }
}
