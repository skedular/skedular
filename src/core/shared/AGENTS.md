# Core Shared Agent Notes

This file covers `core/shared/`.

## Scope

- `core/shared/` owns cross-cutting platform capabilities: user profiles, organization member management, file/media
  management, platform-level settings, and auth-critical replication.
- It is consumed by `Core.Api`, `Core.Jobs`, and `Core.Processors`.

## Replication Boundary

- Core intentionally keeps replicated organization and customer state because many authorization decisions in the core
  domain depend on this replicated data.
- Do not remove auth-critical replicas without a full platform authorization audit.
- Replicated state here is kept fresh via `Core.Processors` Kafka subscriptions.

## File Storage

- Core is the platform owner for user file/media storage via `Enterprise.Shared.FileStorage`.
- File storage is registered in both `Core.Api` and `Core.Jobs`.

## Temporal / Workflow ID Rule

- Core Temporal workflow IDs belong in `core/shared/Core.Shared/Services/WorkflowIdService.cs`.
- Keep workflow ID rules centralized rather than rebuilding them inline at call sites.
- Keep unit tests split one class/file per `WorkflowIdService` method.

## Agent Rule

- Treat `core/shared/` as a dependency surface shared by all other domains through the platform.
- Preserve compatibility and avoid domain-specific shortcuts here.
- Assume replicated auth state may be intentional and required by downstream authorization paths.
