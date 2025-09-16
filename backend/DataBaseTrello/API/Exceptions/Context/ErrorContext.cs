using System.Net;

namespace API.Exceptions.Context
{
    public class ErrorContext
    {
        public string Service { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public HttpStatusCode? StatusCode { get; set; }
        public string LoggerMessage { get; set; } = string.Empty ;

        public ErrorContext(string service, string operation, HttpStatusCode statusCode, string loggerMessage)
        {
            Service = service;
            Operation = operation;
            StatusCode = statusCode;
            LoggerMessage = loggerMessage;
        }
    }
}
