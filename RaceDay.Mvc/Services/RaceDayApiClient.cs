using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace RaceDay.Mvc.Services;

public class RaceDayApiClient
{
    private readonly IHttpClientFactory _factory;
    private readonly IHttpContextAccessor _accessor;

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public RaceDayApiClient(IHttpClientFactory factory, IHttpContextAccessor accessor)
    {
        _factory = factory;
        _accessor = accessor;
    }

    public async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string url,
        object? body = null)
    {
        var client = _factory.CreateClient("RaceDayApi");

        var request =
            new HttpRequestMessage(method, url);

        var cookie = _accessor.HttpContext?.Session.GetString("ApiSessionCookie");
        if (!string.IsNullOrWhiteSpace(cookie))
            request.Headers.TryAddWithoutValidation("Cookie", cookie);

        if (body != null)
        {
            request.Content =
                JsonContent.Create(body);
        }

        return await client.SendAsync(request);
    }

    public static string? ExtractSessionCookie(HttpResponseMessage response)
    {
        if (!response.Headers.TryGetValues("Set-Cookie", out var values)) return null;
        var value = values.FirstOrDefault(v => v.StartsWith(".AspNetCore.Session=", StringComparison.OrdinalIgnoreCase));
        return value?.Split(';', 2)[0];
    }

    public static async Task<T?> ReadAsync<T>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode) return default;
        return JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync(), JsonOptions);
    }
}