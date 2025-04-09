using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Integration.Fixture;
using Ambev.DeveloperEvaluation.ORM;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.Integration.Utils;

public static class TestHelper
{
    public static HttpClient CreateAuthenticatedClient(CustomWebApplicationFactory factory, out DefaultContext context)
    {
        context = CustomWebApplicationFactory.StaticServices.GetRequiredService<DefaultContext>();
        var jwtGenerator = CustomWebApplicationFactory.StaticServices.GetRequiredService<IJwtTokenGenerator>();
        var user = context.Users.First();
        var token = jwtGenerator.GenerateToken(user);

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    public static async Task<JsonDocument> ParseJsonAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(content);
    }

    public static List<string> ExtractErrorDetails(JsonDocument doc)
    {
        return [.. doc.RootElement
                  .GetProperty("errors")
                  .EnumerateArray()
                  .Select(e => e.GetProperty("detail").GetString()!)];
    }

    public static List<string> ExtractErrorFields(JsonDocument doc)
    {
        return [.. doc.RootElement
                  .GetProperty("errors")
                  .EnumerateArray()
                  .Select(e => e.GetProperty("error").GetString()!)];
    }
}
