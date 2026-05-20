using FastEndpoints;
using ResearchAssistant.Api.Infrastructure;

namespace ResearchAssistant.Api.Features.Agent;

public class AgentRequest
{
    public string Question { get; set; } = string.Empty;
}

public class AgentResponse
{
    public string Answer { get; set; } = string.Empty;
}

public class AgentEndpoint : Endpoint<AgentRequest, AgentResponse>
{
    private readonly ILanguageModelService _languageModelService;
    private readonly IEmbeddingService _embeddingService;
    private readonly IDocumentService _documentService;

    public AgentEndpoint(
        ILanguageModelService languageModelService,
        IEmbeddingService embeddingService,
        IDocumentService documentService)
    {
        _languageModelService = languageModelService;
        _embeddingService = embeddingService;
        _documentService = documentService;
    }

    public override void Configure()
    {
        Post("/agent");
        AllowAnonymous();
    }

    public override async Task HandleAsync(AgentRequest req, CancellationToken ct)
    {
        var tools = new List<AgentTool>
        {
            new()
            {
                Name = "search_documents",
                Description = "Searches the knowledge base for relevant information. Use this first to find context.",
                Execute = async input =>
                {
                    var embedding = await _embeddingService.EmbedAsync(input, ct);
                    var results = await _documentService.SearchAsync(embedding, 3, ct);
                    var resultList = results.ToList();
                    return resultList.Count == 0
                        ? "No relevant documents found."
                        : string.Join("\n\n", resultList);
                }
            },
            new()
            {
                Name = "generate_answer",
                Description = "Generates a final answer based on provided context.",
                Execute = async input =>
                {
                    return await _languageModelService.GenerateAsync(input, ct);
                }
            }
        };

        var runner = new AgentRunner(_languageModelService, tools);
        var answer = await runner.RunAsync(req.Question, ct);

        await Send.OkAsync(new AgentResponse { Answer = answer }, ct);
    }
}