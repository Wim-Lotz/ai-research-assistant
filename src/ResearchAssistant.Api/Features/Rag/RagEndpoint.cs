using FastEndpoints;
using ResearchAssistant.Api.Infrastructure;

namespace ResearchAssistant.Api.Features.Rag;

public class RagRequest
{
    public string Question { get; set; } = string.Empty;
    public int ContextLimit { get; set; } = 3;
}

public class RagResponse
{
    public string Answer { get; set; } = string.Empty;
    public IEnumerable<string> SourceContext { get; set; } = [];
}

public class RagEndpoint : Endpoint<RagRequest, RagResponse>
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IDocumentService _documentService;
    private readonly ILanguageModelService _languageModelService;

    public RagEndpoint(
        IEmbeddingService embeddingService,
        IDocumentService documentService,
        ILanguageModelService languageModelService)
    {
        _embeddingService = embeddingService;
        _documentService = documentService;
        _languageModelService = languageModelService;
    }

    public override void Configure()
    {
        Post("/rag");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RagRequest req, CancellationToken ct)
    {
        var queryEmbedding = await _embeddingService.EmbedAsync(req.Question, ct);
        var context = (await _documentService.SearchAsync(queryEmbedding, 
            req.ContextLimit, ct)).ToList();

        if (context.Count == 0)
        {
            await Send.OkAsync(new RagResponse
            {
                Answer = "I don't have information about that in my knowledge base.",
                SourceContext = []
            }, ct);
            return;
        }

        var contextText = string.Join("\n\n", context);

        var prompt = $"""
                      You are a helpful assistant. Answer the question based on the context provided below.
                      Keep your answer concise and relevant to the context.
                      If the context is not relevant to the question, respond with: "I don't have information about that in my knowledge base."

                      Context:
                      {contextText}

                      Question:
                      {req.Question}

                      Answer:
                      """;

        var answer = await _languageModelService.GenerateAsync(prompt, ct);

        await Send.OkAsync(new RagResponse
        {
            Answer = answer,
            SourceContext = context
        }, ct);
    }
}