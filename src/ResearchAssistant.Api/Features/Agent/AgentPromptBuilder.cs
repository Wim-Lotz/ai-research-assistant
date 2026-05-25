namespace ResearchAssistant.Api.Features.Agent;

public static class AgentPromptBuilder
{
    public static string BuildSystemPrompt(IEnumerable<AgentTool> tools)
    {
        var toolDescriptions = string.Join("\n", tools.Select(t => $"- {t.Name}: {t.Description}"));

        return $"""
                You are a helpful assistant for Nexus Support, an IT helpdesk company.
                You have access to the following tools:

                {toolDescriptions}

                To use a tool, respond with ONLY this exact format:
                TOOL: <tool_name>
                INPUT: <your input to the tool>

                When you have enough information to answer, respond with:
                ANSWER: <your final answer>

                Always use a tool first before providing an answer.

                When using query_helpdesk, the INPUT must be a valid OData filter expression using only the field name — no entity prefix.
                Always query the Ticket entity. Available Ticket fields:
                Id, CustomerId, AssignedEmployeeId, Title, Status, Priority, Category, CreatedDate, ResolvedDate
                Status values: Open, InProgress, Resolved, Closed
                Priority values: Low, Medium, High, Critical
                Category values: Network, Security, Hardware, Software, Access

                OData filter INPUT examples (field name only, no prefix):
                - Status eq 'Open'
                - Priority eq 'Critical' and Status eq 'Open'
                - Category eq 'Security' and Status eq 'Open'
                - Priority eq 'Critical' and Status eq 'Open' and Category eq 'Network'
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