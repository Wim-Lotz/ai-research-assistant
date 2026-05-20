using FastEndpoints;
using ResearchAssistant.Api.Infrastructure;

namespace ResearchAssistant.Api.Features.Generate;

public class GenerateRequest
{
    public string Prompt { get; set; } = string.Empty;
}

public class GenerateResponse
{
    public string Response { get; set; } = string.Empty;
}

public class GenerateEndpoint : Endpoint<GenerateRequest, GenerateResponse>
{
    private readonly ILanguageModelService _languageModelService;

    public GenerateEndpoint(ILanguageModelService languageModelService)
    {
        _languageModelService = languageModelService;
    }

    public override void Configure()
    {
        Post("/generate");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GenerateRequest req, CancellationToken ct)
    {
        var result = await _languageModelService.GenerateAsync(req.Prompt, ct);

        await Send.OkAsync(new GenerateResponse { Response = result }, ct);
    }
}