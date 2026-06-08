# Research Assistant

A local AI-powered helpdesk assistant built with C# to learn and demonstrate Generative AI, Vector Databases, RAG, Agentic AI, MCP, and ML.NET.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Ollama](https://ollama.com)

## Getting Started

**1. Install Ollama models**
```bash
ollama pull mistral:7b
ollama pull nomic-embed-text
```

**2. Start infrastructure**
```bash
cd docker && docker compose up -d
```

SQL schema and seed data are applied automatically on first start.

**3. Run the API**
```bash
dotnet run --project src/ResearchAssistant.Api/ResearchAssistant.Api.csproj
```

**4. Run the UI**
```bash
dotnet run --project src/ResearchAssistant.UI/ResearchAssistant.UI.csproj
```

API runs on `http://localhost:5190`

## Configuration

### Ollama (default)
No extra config needed. Uses `mistral:7b` for generation and `nomic-embed-text` for embeddings.

### Azure OpenAI
Set `AI:Provider` to `"AzureOpenAI"` in `appsettings.json` and fill in your resource details:

```json
"AzureOpenAI": {
  "Endpoint": "https://<your-resource>.openai.azure.com/",
  "DeploymentName": "gpt-4o"
}
```

Store your API key in user secrets (never in appsettings.json):
```bash
cd src/ResearchAssistant.Api
dotnet user-secrets set "AzureOpenAI:ApiKey" "<your-key>"
```

Embeddings always use Ollama — switching embedding providers would require re-ingesting all documents with the new vector dimensions.

## Backlog
- [ ] Source attribution (knowledge base vs model training data)
