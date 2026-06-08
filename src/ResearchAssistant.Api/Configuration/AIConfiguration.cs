using Azure;
using Azure.AI.OpenAI;
using OllamaSharp;
using ResearchAssistant.Api.Infrastructure;

namespace ResearchAssistant.Api.Configuration;

public static class AIConfiguration
{
    public static void AddAIConfiguration(this WebApplicationBuilder builder)
    {
        var provider = builder.Configuration["AI:Provider"] ?? "Ollama";

        switch (provider)
        {
            case "AzureOpenAI":
                builder.RegisterAzureOpenAI();
                break;
            case "Ollama":
            default:
                builder.RegisterOllama();
                break;
        }

        builder.Services.AddScoped<IEmbeddingService, EmbeddingService>();
    }

    private static void RegisterOllama(this WebApplicationBuilder builder)
    {
        var ollamaUrl = builder.Configuration["Ollama:BaseUrl"]
                        ?? "http://localhost:11434";

        var modelName = builder.Configuration["Ollama:Model"]
                        ?? "phi3:mini";

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(ollamaUrl),
            Timeout = TimeSpan.FromMinutes(10)
        };
        builder.Services.AddSingleton(_ => new OllamaApiClient(httpClient, modelName));
        builder.Services.AddScoped<ILanguageModelService, OllamaLanguageModelService>();
    }

    private static void RegisterAzureOpenAI(this WebApplicationBuilder builder)
    {
        var endpoint = builder.Configuration["AzureOpenAI:Endpoint"]
                       ?? throw new InvalidOperationException("AzureOpenAI:Endpoint is required");
        var apiKey = builder.Configuration["AzureOpenAI:ApiKey"]
                     ?? throw new InvalidOperationException("AzureOpenAI:ApiKey is required");
        var deploymentName = builder.Configuration["AzureOpenAI:DeploymentName"]
                             ?? throw new InvalidOperationException("AzureOpenAI:DeploymentName is required");

        var azureClient = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
        var chatClient = azureClient.GetChatClient(deploymentName);

        builder.Services.AddSingleton(chatClient);
        builder.Services.AddScoped<ILanguageModelService, AzureOpenAILanguageModelService>();
    }
}