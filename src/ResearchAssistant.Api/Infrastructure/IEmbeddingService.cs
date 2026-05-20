namespace ResearchAssistant.Api.Infrastructure;

public interface IEmbeddingService
{
    Task<float[]> EmbedAsync(string text, CancellationToken ct = default);
}