using OllamaSharp;

namespace ResearchAssistant.Api.Infrastructure;

public class OllamaLanguageModelService : ILanguageModelService
{
    private readonly OllamaApiClient _client;

    public OllamaLanguageModelService(OllamaApiClient client)
    {
        _client = client;
    }

    public async Task<string> GenerateAsync(string prompt, CancellationToken ct = default)
    {
        var response = new System.Text.StringBuilder();

        await foreach (var chunk in _client.GenerateAsync(prompt).WithCancellation(ct))
        {
            if (chunk?.Response is not null)
                response.Append(chunk.Response);
        }

        return response.ToString();
    }
}