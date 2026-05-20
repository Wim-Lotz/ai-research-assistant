# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Tech Stack

C# 12 / .NET 10 REST API using **FastEndpoints** (not minimal APIs or controllers). Semantic search via **Qdrant** (vector DB), structured data via **SQL Server 2022**, local LLM inference via **Ollama**. Microsoft.Extensions.AI provides the LLM abstraction layer.

## Required Setup

Infrastructure (Qdrant + SQL Server) runs in Docker:

```bash
cd docker && docker compose up -d
```

Pull Ollama models before first run:

```bash
ollama pull mistral:7b
ollama pull nomic-embed-text
```

SQL schema and seed data must be applied manually — connect to `localhost:1433` (SA / Research@Assistant123) and run `docker/sql/01_schema.sql` then `docker/sql/02_seed.sql`.

## Running the API

```bash
dotnet run --project src/ResearchAssistant.Api/ResearchAssistant.Api.csproj
```

API listens on `http://localhost:5190`.

## Switching AI Provider

Set `AI:Provider` in `appsettings.json` to `"Ollama"` (default) or `"AzureOpenAI"`. **Azure OpenAI is not implemented** — its service throws `NotImplementedException`. Only Ollama works.

## Code Style

- Use `record` or `record struct` for request/response DTOs and value objects; use `class` for services.
- Do not add comments or XML docs — code should be self-documenting via naming.
- Nullable reference types are enabled; handle nullability explicitly.
- All I/O must be async with `CancellationToken` propagation.

## Commits

Follow Conventional Commits: `feat:`, `fix:`, `chore:`, `refactor:`, `docs:`, `test:`.

## Gotchas

- **Vector size is hardcoded to 768** (`nomic-embed-text` dimensions) in `IEmbeddingService`/Qdrant setup. Changing embedding models requires updating this constant.
- **Agent loops cap at 3 iterations** (`AgentRunner`). If the LLM doesn't converge within 3 tool calls the loop stops and returns a partial answer.
- **No authentication** — all endpoints use `AllowAnonymous`. Do not expose the API publicly.
- There are no tests in this repo.
