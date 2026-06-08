using System.Text.Json;
using FastEndpoints;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using ResearchAssistant.Api.Infrastructure;

namespace ResearchAssistant.Api.Features.Agent;

public class AgentStreamRequest
{
    public string Question { get; set; } = string.Empty;
}

public class AgentStreamEndpoint : Endpoint<AgentStreamRequest>
{
    private static readonly JsonSerializerOptions SseJsonOptions = new(JsonSerializerDefaults.Web);

    private readonly ILanguageModelService _languageModelService;
    private readonly IEmbeddingService _embeddingService;
    private readonly IDocumentService _documentService;
    private readonly McpClient _mcpClient;

    public AgentStreamEndpoint(
        ILanguageModelService languageModelService,
        IEmbeddingService embeddingService,
        IDocumentService documentService,
        McpClient mcpClient)
    {
        _languageModelService = languageModelService;
        _embeddingService = embeddingService;
        _documentService = documentService;
        _mcpClient = mcpClient;
    }

    public override void Configure()
    {
        Post("/agent/stream");
        AllowAnonymous();
    }

    public override async Task HandleAsync(AgentStreamRequest req, CancellationToken ct)
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
                Name = "query_helpdesk",
                Description = "Queries the helpdesk database for structured data about tickets, customers, employees and departments. Use this for questions about specific ticket statuses, priorities, customers or staff.",
                Execute = async input =>
                {
                    var result = await _mcpClient.CallToolAsync(
                        "read_records",
                        new Dictionary<string, object?>
                        {
                            ["entity"] = "Ticket",
                            ["filter"] = input
                        },
                        cancellationToken: ct);

                    var text = result.Content
                        .OfType<TextContentBlock>()
                        .FirstOrDefault()?.Text;

                    return text ?? "No results found.";
                }
            }
        };

        HttpContext.Response.ContentType = "text/event-stream";
        HttpContext.Response.Headers.CacheControl = "no-cache";
        HttpContext.Response.Headers.Connection = "keep-alive";

        await HttpContext.Response.Body.FlushAsync(ct);

        var runner = new AgentRunner(_languageModelService, tools);

        try
        {
            await foreach (var evt in runner.StreamAsync(req.Question, ct))
            {
                var json = JsonSerializer.Serialize(evt, SseJsonOptions);
                await HttpContext.Response.WriteAsync($"data: {json}\n\n", ct);
                await HttpContext.Response.Body.FlushAsync(ct);
            }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            try
            {
                var error = new AgentStreamEvent("error", Token: ex.Message);
                var json = JsonSerializer.Serialize(error, SseJsonOptions);
                await HttpContext.Response.WriteAsync($"data: {json}\n\n", CancellationToken.None);
                await HttpContext.Response.Body.FlushAsync(CancellationToken.None);
            }
            catch
            {
            }
        }
    }
}
