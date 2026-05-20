using ResearchAssistant.Api.Infrastructure;

namespace ResearchAssistant.Api.Features.Agent;

public class AgentRunner
{
    private readonly ILanguageModelService _languageModelService;
    private readonly List<AgentTool> _tools;
    private const int MaxIterations = 3;

    public AgentRunner(ILanguageModelService languageModelService, List<AgentTool> tools)
    {
        _languageModelService = languageModelService;
        _tools = tools;
    }

    public async Task<string> RunAsync(string question, CancellationToken ct = default)
    {
        var systemPrompt = AgentPromptBuilder.BuildSystemPrompt(_tools);
        var userPrompt = AgentPromptBuilder.BuildUserPrompt(question);
        var conversationHistory = $"{systemPrompt}\n\n{userPrompt}";

        for (int i = 0; i < MaxIterations; i++)
        {
            var response = await _languageModelService.GenerateAsync(conversationHistory, ct);

            if (response.Contains("ANSWER:"))
            {
                return response.Split("ANSWER:")[1].Trim();
            }

            if (response.Contains("TOOL:") && response.Contains("INPUT:"))
            {
                var toolName = response.Split("TOOL:")[1].Split("\n")[0].Trim();
                var toolInput = response.Split("INPUT:")[1].Trim();

                var tool = _tools.FirstOrDefault(t => t.Name == toolName);

                if (tool is not null)
                {
                    var toolResult = await tool.Execute(toolInput);
                    conversationHistory += $"\n\n{response}\n\n{AgentPromptBuilder.BuildToolResultPrompt(toolName, toolResult)}";
                }
                else
                {
                    conversationHistory += $"\n\nTool '{toolName}' not found. Available tools: {string.Join(", ", _tools.Select(t => t.Name))}";
                }

                continue;
            }

            // LLM didn't follow format - nudge it
            conversationHistory += $"\n\n{response}\n\nPlease use the TOOL/INPUT format or provide an ANSWER:";
        }

        return "I was unable to answer your question after multiple attempts.";
    }
}