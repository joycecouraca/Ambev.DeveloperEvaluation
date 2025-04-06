namespace Ambev.DeveloperEvaluation.Application.Common.Response;

public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}
