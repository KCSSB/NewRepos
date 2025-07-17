using System.Net;

namespace API.Exceptions.ErrorContext
{
    public class ErrorContext
    {
        public string Service { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;

        public HttpStatusCode StatusCode { get; set; }
        
        public string UserMessage { get; set; } = string.Empty ;
        public string LoggerMessage { get; set; } = string.Empty ;

        public ErrorContext(string service, string operation, HttpStatusCode statusCode, string userMessage, string loggerMessage)
        {
            Service = service;
            Operation = operation;
            StatusCode = statusCode;
            UserMessage = userMessage;
            LoggerMessage = loggerMessage;
        }
    }
}
