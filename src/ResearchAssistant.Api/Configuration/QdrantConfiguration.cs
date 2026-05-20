using Qdrant.Client;
using ResearchAssistant.Api.Infrastructure;

namespace ResearchAssistant.Api.Configuration;

public static class QdrantConfiguration
{
    public static void AddQdrantConfiguration(this WebApplicationBuilder builder)
    {
        var host = builder.Configuration["Qdrant:Host"] ?? "localhost";
        var port = int.Parse(builder.Configuration["Qdrant:Port"] ?? "6334");

        builder.Services.AddSingleton(_ => new QdrantClient(host, port));
        builder.Services.AddScoped<IDocumentService, DocumentService>();
    }
}