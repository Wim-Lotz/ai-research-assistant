namespace ResearchAssistant.Api.Features.Agent;

public record AgentStreamEvent(string Type, string? Tool = null, string? Input = null, string? Result = null, string? Token = null);
