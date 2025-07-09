namespace API.Errors;

public class ApiException(string message, int statusCode, string? details)
{
    public string Message { get; set; } = message;
    public int StatusCode { get; set; } = statusCode;
    public string? Details { get; set; } = details;
}
