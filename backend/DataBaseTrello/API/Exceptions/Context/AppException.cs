namespace API.Exceptions.Context
{
    public class AppException: Exception
    {
        public ErrorContext Context { get; }

        public AppException(ErrorContext context,Exception? innerException = null): base(context?.LoggerMessage, innerException)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }
    }
}
