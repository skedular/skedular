<!-- SPECKIT START -->
For additional context about technologies to be used, project structure,
shell commands, and other important information, read the current plan

Current feature: 032-unified-host-listing (Unified Host Listing Experience)
Plan: specs/032-unified-host-listing/plan.md
<!-- SPECKIT END -->

## Active Technologies
- C# .NET 10 (backend); TypeScript 6 / React 19 / Next.js 16 App Router (frontend) (007-resource-availability-dashboard)
- PostgreSQL via EF Core — no new migration required for the query path; the existing `DailyResourceAvailabilitySnapshot` table (from 006) supports analytics but the dashboard queries live booking data at request time via gRPC (007-resource-availability-dashboard)

## Recent Changes
- 032-unified-host-listing: Plan created for unified Host Location/Product experience (Phase 1 complete)
- 007-resource-availability-dashboard: Added C# .NET 10 (backend); TypeScript 6 / React 19 / Next.js 16 App Router (frontend)

## graphify

This project has a knowledge graph at graphify-out/ with god nodes, community structure, and cross-file relationships.

Rules:
- For codebase questions, first run `graphify query "<question>"` when graphify-out/graph.json exists. Use `graphify path "<A>" "<B>"` for relationships and `graphify explain "<concept>"` for focused concepts. These return a scoped subgraph, usually much smaller than GRAPH_REPORT.md or raw grep output.
- If graphify-out/wiki/index.md exists, use it for broad navigation instead of raw source browsing.
- Read graphify-out/GRAPH_REPORT.md only for broad architecture review or when query/path/explain do not surface enough context.
- After modifying code, run `graphify update .` to keep the graph current (AST-only, no API cost).
