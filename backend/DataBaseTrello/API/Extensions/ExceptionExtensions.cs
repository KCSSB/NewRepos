namespace API.Extensions
{
    public static class ExceptionExtensions
    {
        public static bool TryGetFromExceptionData<T>(
    this Exception ex,
    string key,
    out T value)
        {
            if (ex.Data.Contains(key) && ex.Data[key] is T typedValue)
            {
                value = typedValue;
                return true;
            }

            value = default!;
            return false;
        }
    }
}
