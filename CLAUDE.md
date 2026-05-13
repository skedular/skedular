<!-- SPECKIT START -->
For additional context about technologies to be used, project structure,
shell commands, and other important information, read the current plan
<!-- SPECKIT END -->

## Active Technologies
- C# .NET 10 (backend); TypeScript 6 / React 19 / Next.js 16 App Router (frontend) (007-resource-availability-dashboard)
- PostgreSQL via EF Core — no new migration required for the query path; the existing `DailyResourceAvailabilitySnapshot` table (from 006) supports analytics but the dashboard queries live booking data at request time via gRPC (007-resource-availability-dashboard)

## Recent Changes
- 007-resource-availability-dashboard: Added C# .NET 10 (backend); TypeScript 6 / React 19 / Next.js 16 App Router (frontend)
