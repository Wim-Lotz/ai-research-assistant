using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace ResearchAssistant.Api.Infrastructure;

public class DocumentService : IDocumentService
{
    private readonly QdrantClient _client;
    private readonly string _collectionName;
    private readonly float _scoreThreshold;
    private const int VectorSize = 768;

    public DocumentService(QdrantClient client, IConfiguration configuration)
    {
        _client = client;
        _collectionName = configuration["Qdrant:CollectionName"] ?? "documents";
        _scoreThreshold = float.Parse(configuration["Qdrant:ScoreThreshold"] ?? "0.5");
    }

    public async Task InitialiseAsync(CancellationToken ct = default)
    {
        var collections = await _client.ListCollectionsAsync(ct);
        var exists = collections.Any(c => c == _collectionName);

        if (!exists)
        {
            await _client.CreateCollectionAsync(
                collectionName: _collectionName,
                vectorsConfig: new VectorParams
                {
                    Size = VectorSize,
                    Distance = Distance.Cosine
                },
                cancellationToken: ct);
        }
    }

    public async Task StoreAsync(string documentId, string text, float[] embedding, CancellationToken ct = default)
    {
        var points = new List<PointStruct>
        {
            new()
            {
                Id = new PointId { Uuid = documentId },
                Vectors = embedding,
                Payload =
                {
                    ["text"] = text
                }
            }
        };

        await _client.UpsertAsync(_collectionName, points, cancellationToken: ct);
    }

    public async Task StoreAsync(string documentId, string text, float[] embedding, Dictionary<string, string> metadata, CancellationToken ct = default)
    {
        var point = new PointStruct
        {
            Id = new PointId { Uuid = documentId },
            Vectors = embedding,
            Payload = { ["text"] = text }
        };

        foreach (var (key, value) in metadata)
            point.Payload[key] = value;

        await _client.UpsertAsync(_collectionName, [point], cancellationToken: ct);
    }

    public async Task<IEnumerable<string>> SearchAsync(float[] queryEmbedding, int limit = 5, CancellationToken ct = default)
    {
        var results = await _client.SearchAsync(
            _collectionName,
            queryEmbedding,
            limit: (ulong)limit,
            cancellationToken: ct);

        return results
            .Where(r => r.Score >= _scoreThreshold)
            .Select(r => r.Payload["text"].StringValue);
    }
}