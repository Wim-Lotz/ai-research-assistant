using FastEndpoints;
using ResearchAssistant.Api.Infrastructure;

namespace ResearchAssistant.Api.Features.Ingest;

public class RawIngestRequest
{
    public string Text { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
}

public class RawIngestResponse
{
    public string DocumentId { get; set; } = string.Empty;
    public string PredictedType { get; set; } = string.Empty;
}

public class RawIngestEndpoint : Endpoint<RawIngestRequest, RawIngestResponse>
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IDocumentService _documentService;
    private readonly IClassifierService _classifierService;

    public RawIngestEndpoint(
        IEmbeddingService embeddingService,
        IDocumentService documentService,
        IClassifierService classifierService)
    {
        _embeddingService = embeddingService;
        _documentService = documentService;
        _classifierService = classifierService;
    }

    public override void Configure()
    {
        Post("/ingest/raw");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RawIngestRequest req, CancellationToken ct)
    {
        await _documentService.InitialiseAsync(ct);

        var predictedType = _classifierService.Predict(req.Text);
        var documentId = Guid.NewGuid().ToString();
        var embedding = await _embeddingService.EmbedAsync(req.Text, ct);
        var metadata = new Dictionary<string, string>
        {
            ["type"] = predictedType,
            ["source"] = req.Source
        };

        await _documentService.StoreAsync(documentId, req.Text, embedding, metadata, ct);

        await Send.OkAsync(new RawIngestResponse
        {
            DocumentId = documentId,
            PredictedType = predictedType
        }, ct);
    }
}
