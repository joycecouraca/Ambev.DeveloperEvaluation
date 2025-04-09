using System.Net.Http.Json;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.Integration.Utils;

public static class HttpExtensions
{
    public static async Task<T> ReadAsJsonAsync<T>(this HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public static Task<HttpResponseMessage> PostAsJson<T>(this HttpClient client, string url, T data)
        => client.PostAsJsonAsync(url, data);
}
