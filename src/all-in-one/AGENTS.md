# All-In-One Agent Notes

This file is the entry point for AI agents working in `all-in-one/`.

## Purpose

- `all-in-one/` contains composite Aspire projects that run all domain services together in a single process or a
  simplified set of processes.
- Useful for local development, demos, and integration scenarios where running every domain separately is impractical.

## Projects

| Project           | Purpose                                                                      |
|-------------------|------------------------------------------------------------------------------|
| `AllInOne`        | Runs all domain infrastructure migrations simultaneously at startup          |
| `AllApis`         | Runs all domain API hosts in a single process                                |
| `AllApisJobs`     | Runs all domain API + jobs hosts together                                    |
| `AllJobs`         | Runs all domain jobs hosts together                                          |
| `AllProcessors`   | Runs all domain processor (Kafka subscriber) hosts together                  |
| `AllInfra`        | Runs all domain infrastructure (migration) hosts together                    |

## Agent Rule

- Do not add domain business logic to these composite projects.
- These projects should only reference and compose the existing domain hosts.
- If a new domain host is added, add it to the relevant composite projects here too.
- Keep these in sync with the actual domain host set so that `all-in-one` accurately reflects the full system.
