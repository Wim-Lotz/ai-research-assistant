using System.Runtime.CompilerServices;
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

    public async IAsyncEnumerable<AgentStreamEvent> StreamAsync(string question, [EnumeratorCancellation] CancellationToken ct = default)
    {
        var systemPrompt = AgentPromptBuilder.BuildSystemPrompt(_tools);
        var userPrompt = AgentPromptBuilder.BuildUserPrompt(question);
        var conversationHistory = $"{systemPrompt}\n\n{userPrompt}";
        var hadToolResults = false;

        for (int i = 0; i < MaxIterations; i++)
        {
            var response = await _languageModelService.GenerateAsync(conversationHistory, ct);

            bool hasAnswer = response.Contains("ANSWER:");
            bool hasTool = response.Contains("TOOL:") && response.Contains("INPUT:");
            bool answerFirst = hasAnswer && (!hasTool || response.LastIndexOf("ANSWER:") > response.LastIndexOf("TOOL:"));

            if (answerFirst)
            {
                yield return new AgentStreamEvent("token", Token: response.Split("ANSWER:", 2)[1].Trim());
                yield return new AgentStreamEvent("done");
                yield break;
            }

            if (hasTool)
            {
                var toolName = ExtractToolName(response);
                var toolInput = ExtractToolInput(response);

                yield return new AgentStreamEvent("tool_call", Tool: toolName, Input: toolInput);

                var tool = _tools.FirstOrDefault(t => t.Name == toolName);
                string toolResult;

                if (tool is not null)
                {
                    toolResult = await tool.Execute(toolInput);
                    conversationHistory += $"\n\n{response}\n\n{AgentPromptBuilder.BuildToolResultPrompt(toolName, toolResult)}";
                    hadToolResults = true;
                }
                else
                {
                    toolResult = $"Tool '{toolName}' not found. Available tools: {string.Join(", ", _tools.Select(t => t.Name))}";
                    conversationHistory += $"\n\n{response}\n\n{AgentPromptBuilder.BuildToolResultPrompt(toolName, toolResult)}";
                }

                yield return new AgentStreamEvent("tool_result", Tool: toolName, Result: toolResult);
                continue;
            }

            conversationHistory += $"\n\n{response}\n\nPlease use the TOOL/INPUT format or provide an ANSWER:";
        }

        if (hadToolResults)
        {
            var synthesis = await _languageModelService.GenerateAsync(
                conversationHistory + "\n\nYou have gathered enough information. Please provide your final ANSWER: now.",
                ct);
            var answer = synthesis.Contains("ANSWER:")
                ? synthesis.Split("ANSWER:", 2)[1].Trim()
                : "I was unable to synthesize an answer from the gathered information.";
            yield return new AgentStreamEvent("token", Token: answer);
        }
        else
        {
            yield return new AgentStreamEvent("token", Token: "I was unable to answer your question after multiple attempts.");
        }

        yield return new AgentStreamEvent("done");
    }

    public async Task<string> RunAsync(string question, CancellationToken ct = default)
    {
        var systemPrompt = AgentPromptBuilder.BuildSystemPrompt(_tools);
        var userPrompt = AgentPromptBuilder.BuildUserPrompt(question);
        var conversationHistory = $"{systemPrompt}\n\n{userPrompt}";
        var hadToolResults = false;

        for (int i = 0; i < MaxIterations; i++)
        {
            var response = await _languageModelService.GenerateAsync(conversationHistory, ct);

            bool hasAnswer = response.Contains("ANSWER:");
            bool hasTool = response.Contains("TOOL:") && response.Contains("INPUT:");
            bool answerFirst = hasAnswer && (!hasTool || response.LastIndexOf("ANSWER:") > response.LastIndexOf("TOOL:"));

            if (answerFirst)
            {
                return response.Split("ANSWER:", 2)[1].Trim();
            }

            if (hasTool)
            {
                var toolName = ExtractToolName(response);
                var toolInput = ExtractToolInput(response);

                var tool = _tools.FirstOrDefault(t => t.Name == toolName);

                if (tool is not null)
                {
                    var toolResult = await tool.Execute(toolInput);
                    conversationHistory +=
                        $"\n\n{response}\n\n{AgentPromptBuilder.BuildToolResultPrompt(toolName, toolResult)}";
                    hadToolResults = true;
                }
                else
                {
                    var toolResult = $"Tool '{toolName}' not found. Available tools: {string.Join(", ", _tools.Select(t => t.Name))}";
                    conversationHistory +=
                        $"\n\n{response}\n\n{AgentPromptBuilder.BuildToolResultPrompt(toolName, toolResult)}";
                }

                continue;
            }

            conversationHistory += $"\n\n{response}\n\nPlease use the TOOL/INPUT format or provide an ANSWER:";
        }

        if (hadToolResults)
        {
            var synthesis = await _languageModelService.GenerateAsync(
                conversationHistory + "\n\nYou have gathered enough information. Please provide your final ANSWER: now.",
                ct);
            return synthesis.Contains("ANSWER:")
                ? synthesis.Split("ANSWER:", 2)[1].Trim()
                : "I was unable to synthesize an answer from the gathered information.";
        }

        return "I was unable to answer your question after multiple attempts.";
    }

    private static string ExtractToolName(string response)
    {
        var index = response.LastIndexOf("TOOL:");
        return response[(index + "TOOL:".Length)..].Split(new[] { '\n', '\r' }, 2)[0].Trim();
    }

    private static string ExtractToolInput(string response)
    {
        var index = response.LastIndexOf("INPUT:");
        var afterInput = response[(index + "INPUT:".Length)..];
        var firstLine = afterInput.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                                  .FirstOrDefault()?.Trim() ?? string.Empty;
        return firstLine.Contains("ANSWER:")
            ? firstLine.Split("ANSWER:")[0].Trim()
            : firstLine;
    }
}
