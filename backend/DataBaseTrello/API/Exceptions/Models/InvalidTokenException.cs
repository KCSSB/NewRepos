namespace API.Exceptions.Models
{
    public class InvalidTokenException: Exception
    {
        public string Service { get; set; } = string.Empty;

        public string Operation { get; set; } = string.Empty;

        public InvalidTokenException(string message, string service, string operation):base(message)
        {
            Service = service;
            Operation = operation;
        }

    }
}
