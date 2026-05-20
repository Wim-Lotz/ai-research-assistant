using FastEndpoints;
using ResearchAssistant.Api.Infrastructure;

namespace ResearchAssistant.Api.Features.Ingest;

public class IngestRequest
{
    public string Text { get; set; } = string.Empty;
}

public class IngestResponse
{
    public string DocumentId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class IngestEndpoint : Endpoint<IngestRequest, IngestResponse>
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IDocumentService _documentService;

    public IngestEndpoint(IEmbeddingService embeddingService, IDocumentService documentService)
    {
        _embeddingService = embeddingService;
        _documentService = documentService;
    }

    public override void Configure()
    {
        Post("/ingest");
        AllowAnonymous();
    }

    public override async Task HandleAsync(IngestRequest req, CancellationToken ct)
    {
        await _documentService.InitialiseAsync(ct);

        var documentId = Guid.NewGuid().ToString();
        var embedding = await _embeddingService.EmbedAsync(req.Text, ct);
        await _documentService.StoreAsync(documentId, req.Text, embedding, ct);

        await Send.OkAsync(new IngestResponse
        {
            DocumentId = documentId,
            Message = "Document ingested successfully"
        }, ct);
    }
}