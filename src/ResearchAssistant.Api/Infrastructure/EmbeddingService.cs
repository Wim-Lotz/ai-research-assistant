using OllamaSharp;
using OllamaSharp.Models;

namespace ResearchAssistant.Api.Infrastructure;

public class EmbeddingService : IEmbeddingService
{
    private readonly OllamaApiClient _client;
    private readonly string _embeddingModel;

    public EmbeddingService(OllamaApiClient client, IConfiguration configuration)
    {
        _client = client;
        _embeddingModel = configuration["Ollama:EmbeddingModel"] ?? "nomic-embed-text";
    }

    public async Task<float[]> EmbedAsync(string text, CancellationToken ct = default)
    {
        var response = await _client.EmbedAsync(new EmbedRequest
        {
            Model = _embeddingModel,
            Input = [text]
        }, ct);

        return response.Embeddings[0];
    }
}