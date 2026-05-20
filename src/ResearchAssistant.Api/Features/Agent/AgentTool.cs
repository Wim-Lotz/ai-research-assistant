namespace ResearchAssistant.Api.Features.Agent;

public class AgentTool
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Func<string, Task<string>> Execute { get; set; } = _ => Task.FromResult(string.Empty);
}