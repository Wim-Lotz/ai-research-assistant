using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;

namespace ResearchAssistant.UI.Services;

public record AgentStreamDto(string Type, string? Tool, string? Input, string? Result, string? Token);

public class AgentStreamClient
{
    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public AgentStreamClient(string baseUrl)
    {
        _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public async IAsyncEnumerable<AgentStreamDto> StreamAsync(string question, [EnumeratorCancellation] CancellationToken ct = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/agent/stream")
        {
            Content = JsonContent.Create(new { question })
        };

        using var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var reader = new StreamReader(stream);

        string? line;
        while ((line = await reader.ReadLineAsync(ct)) is not null)
        {
            if (!line.StartsWith("data: "))
                continue;

            var json = line["data: ".Length..];
            var evt = JsonSerializer.Deserialize<AgentStreamDto>(json, JsonOptions);

            if (evt is not null)
                yield return evt;
        }
    }
}
