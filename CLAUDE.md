# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Developer Profile

- Senior C# backend developer — strong .NET background, no need to explain language basics.
- New to AI concepts — explain AI patterns, terminology, and design decisions thoroughly.
- Prefers to understand every line of code — do not generate large blocks without explanation.
- No vibe coding — code quality and understanding over speed.
- Self-documenting code — no comments or XML docs needed.

## Project Purpose

This is a teaching and portfolio project. The goal is to learn the full AI engineering stack to a level where the developer can help companies implement AI solutions. Breadth of AI concepts matters more than polish. Always explain *why* a pattern exists and what problem it solves in a real company context, not just how to implement it.

**What has been learned so far:**
- **RAG** — the most common pattern companies ask for: "we have documents, we want to query them with AI."
- **ReAct agentic loops** — how you go from a single LLM call to an AI that can reason and act over multiple steps.

**What each remaining roadmap item teaches:**
- **MCP Server + Azure OpenAI** — MCP is the emerging standard for giving agents access to tools and data sources. Azure OpenAI is what most enterprises actually use. Together they teach hybrid retrieval: combining structured SQL queries with semantic vector search.
- **ML.NET classifier** — the distinction between *using* AI (calling an LLM API) and *understanding* ML (training models, feature engineering, inference pipelines). Teaches how a trained model integrates into a data pipeline.
- **Avalonia UI** — building what the business stakeholder actually sees; also teaches streaming responses.
- **Source attribution** — a real enterprise requirement: did this answer come from the knowledge base or the model's training data? Relevant for compliance and audit trails.
- **gRPC** — how AI services communicate in a microservices architecture at large companies.

**Suggested build order:** MCP Server + Azure OpenAI → ML.NET classifier → Avalonia UI → source attribution + gRPC.

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

## Architecture

Vertical slice architecture under `/Features`. Each feature folder contains everything for that feature (endpoint, request, response, any feature-specific services). Shared AI infrastructure lives in `/Infrastructure`.

**Do not** reorganise into Clean Architecture layers (no `/Controllers`, `/Services`, `/Repositories` folders).

## What Has Been Built

- GET `/health` — health check
- POST `/generate` — raw LLM generation
- POST `/ingest` — ingest text into Qdrant
- POST `/ingest/movies` — ingest movies from SQL Server into Qdrant
- POST `/search` — semantic search against Qdrant
- POST `/rag` — full RAG pipeline (embed → search → augment → generate)
- POST `/agent` — ReAct agentic loop with tool calling

## Remaining Roadmap

1. **MCP Server** — DAB (Data API Builder) in Docker on top of SQL Server, MCP server in Docker, new agent tool `query_movies_database`
2. **ML.NET** — document classifier on ingestion
3. **Avalonia UI** — desktop chat interface wired to REST API
4. **Azure OpenAI** — implement `AzureOpenAILanguageModelService`

## Backlog

- Agent response should indicate source (knowledge base vs training knowledge)
- Clean old test documents from Qdrant (machine learning, vector db, docker text entries)
- Add gRPC endpoints alongside REST

## Data

SQL Server contains 15 movies with directors, actors, genres, reviews.
Movies are also ingested into Qdrant with rich text for semantic search.
This enables both structured queries (via MCP/SQL) and semantic queries (via RAG/Qdrant).

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
