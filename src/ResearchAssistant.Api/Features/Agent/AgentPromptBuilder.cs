namespace ResearchAssistant.Api.Features.Agent;

public static class AgentPromptBuilder
{
    public static string BuildSystemPrompt(IEnumerable<AgentTool> tools)
    {
        var toolDescriptions = string.Join("\n", tools.Select(t => $"- {t.Name}: {t.Description}"));

        return $"""
                You are a helpful research assistant with access to the following tools:

                {toolDescriptions}

                To use a tool, respond with ONLY this exact format:
                TOOL: <tool_name>
                INPUT: <your input to the tool>

                When you have enough information to answer, respond with:
                ANSWER: <your final answer>

                Always use a tool first before providing an answer.
                """;
    }

    public static string BuildUserPrompt(string question)
    {
        return $"Question: {question}";
    }

    public static string BuildToolResultPrompt(string toolName, string result)
    {
        return $"""
                Tool '{toolName}' returned:
                {result}

                Now either use another tool or provide your final ANSWER:
                """;
    }
}