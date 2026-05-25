using ModelContextProtocol.Client;

namespace ResearchAssistant.Api.Configuration;

public static class McpConfiguration
{
    public static void AddMcpConfiguration(this WebApplicationBuilder builder)
    {
        var endpoint = builder.Configuration["Mcp:Endpoint"]
            ?? throw new InvalidOperationException("Mcp:Endpoint is not configured.");

        var transport = new HttpClientTransport(new HttpClientTransportOptions
        {
            Endpoint = new Uri(endpoint)
        });

        builder.Services.AddSingleton<McpClient>(_ =>
            McpClient.CreateAsync(transport).GetAwaiter().GetResult()
        );
    }
}
