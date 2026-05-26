namespace ResearchAssistant.Api.Infrastructure;

public interface IDocumentService
{
    Task InitialiseAsync(CancellationToken ct = default);
    Task StoreAsync(string documentId, string text, float[] embedding, CancellationToken ct = default);
    Task StoreAsync(string documentId, string text, float[] embedding, Dictionary<string, string> metadata, CancellationToken ct = default);
    Task<IEnumerable<string>> SearchAsync(float[] queryEmbedding, int limit = 5, CancellationToken ct = default);
}