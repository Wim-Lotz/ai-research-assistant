using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FastEndpoints;
using ResearchAssistant.Api.Infrastructure;

namespace ResearchAssistant.Api.Features.Ingest;

public class HelpdeskIngestResponse
{
    public int DocumentsIngested { get; set; }
}

public class HelpdeskIngestEndpoint : EndpointWithoutRequest<HelpdeskIngestResponse>
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IDocumentService _documentService;

    public HelpdeskIngestEndpoint(IEmbeddingService embeddingService, IDocumentService documentService)
    {
        _embeddingService = embeddingService;
        _documentService = documentService;
    }

    public override void Configure()
    {
        Post("/ingest/helpdesk");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await _documentService.InitialiseAsync(ct);

        var path = Path.Combine(AppContext.BaseDirectory, "Data", "helpdesk-documents.json");
        var json = await File.ReadAllTextAsync(path, ct);
        var seed = JsonSerializer.Deserialize<HelpdeskSeedData>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        foreach (var doc in seed.Documents)
        {
            var text = $"{doc.Title}\n\n{doc.Content}";
            var embedding = await _embeddingService.EmbedAsync(text, ct);
            var metadata = new Dictionary<string, string>
            {
                ["type"] = doc.Type,
                ["category"] = doc.Category,
                ["title"] = doc.Title
            };
            var documentId = new Guid(MD5.HashData(Encoding.UTF8.GetBytes(doc.Id))).ToString();
            await _documentService.StoreAsync(documentId, text, embedding, metadata, ct);
        }

        await Send.OkAsync(new HelpdeskIngestResponse { DocumentsIngested = seed.Documents.Count }, ct);
    }
}

file record HelpdeskSeedData
{
    public List<HelpdeskDocument> Documents { get; init; } = [];
}

file record HelpdeskDocument
{
    public string Id { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
}
