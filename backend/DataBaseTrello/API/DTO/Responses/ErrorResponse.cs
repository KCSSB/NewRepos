using System.Net;
using System.Text.Json;

namespace API.DTO.Responses
{
    public record ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
