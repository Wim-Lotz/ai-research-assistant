using FastEndpoints;
using ResearchAssistant.Api.Infrastructure;

namespace ResearchAssistant.Api.Features.Search;

public class SearchRequest
{
    public string Query { get; set; } = string.Empty;
    public int Limit { get; set; } = 5;
}

public class SearchResponse
{
    public IEnumerable<string> Results { get; set; } = [];
}

public class SearchEndpoint : Endpoint<SearchRequest, SearchResponse>
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IDocumentService _documentService;

    public SearchEndpoint(IEmbeddingService embeddingService, IDocumentService documentService)
    {
        _embeddingService = embeddingService;
        _documentService = documentService;
    }

    public override void Configure()
    {
        Post("/search");
        AllowAnonymous();
    }

    public override async Task HandleAsync(SearchRequest req, CancellationToken ct)
    {
        var queryEmbedding = await _embeddingService.EmbedAsync(req.Query, ct);
        var results = await _documentService.SearchAsync(queryEmbedding, 
            req.Limit, ct);
        await Send.OkAsync(new SearchResponse { Results = results }, ct);
    }
}