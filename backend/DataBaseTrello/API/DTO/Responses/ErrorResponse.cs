using System.Net;
using System.Text.Json;

namespace API.DTO.Responses
{
    public record ErrorResponse
    {
        string Message { get; set; } = string.Empty;
        HttpStatusCode StatusCode { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
