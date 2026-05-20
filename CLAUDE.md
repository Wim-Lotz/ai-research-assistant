# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Developer Profile

- Senior C# backend developer — strong .NET background, no need to explain language basics.
- New to AI concepts — explain AI patterns, terminology, and design decisions thoroughly.
- Prefers to understand every line of code — do not generate large blocks without explanation.
- No vibe coding — code quality and understanding over speed.
- Self-documenting code — no comments or XML docs needed.

## Setup Philosophy

**The entire project must be runnable with only Docker Desktop and the .NET SDK installed.** No manual database steps, no pre-configured external services, no undocumented prerequisites.

This means:
- All infrastructure (Qdrant, SQL Server, MCP server, DAB) runs in Docker Compose
- SQL schema and seed data execute automatically on first `docker compose up` — never require manual script execution
- Ollama model pulls are documented as a single copy-paste block
- The README must be good enough that someone who has never seen the project can clone it and have it running within minutes

**The current manual SQL setup step violates this** — `docker/sql/01_schema.sql` and `02_seed.sql` must be wired into Docker Compose init scripts before the project is considered complete.

The README is written last, once the full stack is built, and covers: prerequisites, one-command startup, what each component does, and example queries including the Altered Carbon-style query.

## Project Purpose

This is a teaching and portfolio project. The goal is to learn the full AI engineering stack to a level where the developer can help companies implement AI solutions. Breadth of AI concepts matters more than polish. Always explain *why* a pattern exists and what problem it solves in a real company context, not just how to implement it.

**What has been learned so far:**
- **RAG** — the most common pattern companies ask for: "we have documents, we want to query them with AI."
- **ReAct agentic loops** — how you go from a single LLM call to an AI that can reason and act over multiple steps.

**What each remaining roadmap item teaches:**
- **MCP Server** — the emerging standard for giving agents access to tools and data sources. Teaches structured SQL queries from an agent, and hybrid retrieval when combined with Qdrant semantic search.
- **Unstructured data ingestion** — sourcing real sci-fi reviews and articles to stress-test the semantic search side of the stack. Structured SQL data (movies) does not require semantic search — unstructured text does.
- **ML.NET classifier** — classifying ingested unstructured documents on their way into Qdrant (e.g. "is this a review, an article, a plot summary?"). Only needed once unstructured data exists — ML classification adds nothing when data already has structured metadata.
- **Azure OpenAI** — swapping Ollama for the provider enterprises actually use. Teaches provider abstraction and cloud LLM integration.
- **Avalonia UI** — building what the business stakeholder actually sees; also teaches streaming responses.
- **Source attribution** — a real enterprise requirement: did this answer come from the knowledge base or the model's training data? Relevant for compliance and audit trails.
- **gRPC** — how AI services communicate in a microservices architecture at large companies.

**The motivating query** (what this stack will be able to answer when complete): *"Give me sci-fi movies from the last 20 years with good reviews — cyberpunk style, like Altered Carbon."* This requires SQL (genre, year, rating) + semantic search (themes, tone, style) + ML classification (scoping search to the right document types) all working together through the agent.

**Build order:** MCP Server → unstructured data ingestion → ML.NET classifier → Azure OpenAI → Avalonia UI → source attribution + gRPC.

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
2. **Unstructured data ingestion** — source real sci-fi reviews and articles (e.g. Roger Ebert reviews, Wikipedia plot summaries), ingest into Qdrant
3. **ML.NET classifier** — classify unstructured documents on ingestion (review vs article vs plot summary) so semantic search can be scoped by document type
4. **Azure OpenAI** — implement `AzureOpenAILanguageModelService`
5. **Avalonia UI** — desktop chat interface wired to REST API

## Backlog

- Agent response should indicate source (knowledge base vs training knowledge)
- Clean old test documents from Qdrant (machine learning, vector db, docker text entries)
- Add gRPC endpoints alongside REST

## Data

### Current (dev/demo only)
SQL Server contains 15 movies with directors, actors, genres, reviews. Too small to be genuinely useful — exists only to validate the pipeline.

### Target dataset (production-scale)
The goal is a dataset large enough to actually use as a personal movie/TV assistant.

**Structured data → SQL Server via IMDb datasets** (`datasets.imdb.com`)
- Free, non-commercial use, downloadable TSV files
- Covers movies and TV: titles, genres, ratings, cast, crew, release years
- Millions of entries — replaces the current 15-movie seed data

**Unstructured text → Qdrant via CMU Movie Summary Corpus**
- ~42,000 Wikipedia plot summaries, public domain research dataset
- Long-form text covering plot, themes, tone — enables semantic queries like "cyberpunk style, like Altered Carbon"
- Supplement with Wikipedia API for TV series and newer titles not in the corpus

**TMDB API** as a supplement for richer metadata (overviews, poster URLs, streaming availability) if needed.

### Why both stores
SQL handles exact structured queries ("sci-fi movies after 2005 with rating > 7"). Qdrant handles semantic queries ("cyberpunk themes, dystopian future, noir aesthetic"). The agent uses both and combines the results.

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
