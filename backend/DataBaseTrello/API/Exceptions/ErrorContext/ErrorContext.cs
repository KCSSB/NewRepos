namespace API.Exceptions.ErrorContext
{
    public class ErrorContext
    {
        public string Service { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;

        public ErrorContext(string service, string operation)
        {
            Service = service;
            Operation = operation;
        }
    }
}
