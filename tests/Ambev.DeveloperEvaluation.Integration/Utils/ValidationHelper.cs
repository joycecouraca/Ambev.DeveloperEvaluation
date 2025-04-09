using System.Text.Json;

namespace Ambev.DeveloperEvaluation.Integration.Utils;

public static class ValidationHelper
{
    public static async Task<List<string>> GetValidationDetailsAsync(HttpResponseMessage response)
    {
        var responseBody = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(responseBody);
        var errors = doc.RootElement.GetProperty("errors");

        return [.. errors.EnumerateArray().Select(e => e.GetProperty("detail").GetString()!)];
    }
    public static async Task<List<string>> GetValidationErrorsAsync(HttpResponseMessage response)
    {
        var responseBody = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(responseBody);
        var errors = doc.RootElement.GetProperty("errors");

        return [.. errors.EnumerateArray().Select(e => e.GetProperty("error").GetString()!)];
    }
}
