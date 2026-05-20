namespace ResearchAssistant.Api.Infrastructure;

public interface ILanguageModelService
{
    Task<string> GenerateAsync(string prompt, CancellationToken ct = default);
}