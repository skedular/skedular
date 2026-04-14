# Quickstart: Implement Team Domain Structured Logging

## 1. Prerequisites

- Branch checked out: `001-team-domain-logging`
- .NET SDK installed for repo target (`.NET 10`)
- Dependencies available for Team test projects

## 1.1 Phase Execution Sequence

1. Setup and foundational checks (T001-T007)
2. US1 implementation and validation (T008-T019)
3. US2 implementation and validation (T020-T027)
4. US3 implementation and validation (T028-T031)
5. US4 implementation and validation (T032-T038)
6. Cross-cutting polish and final verification (T039-T042)

## 1.2 Host Bootstrap Verification Checkpoint

Confirm logging bootstrap remains unchanged in:

- `team/apis/Team.Api/Program.cs`
- `team/jobs/Team.Jobs/Program.cs`
- `team/processors/Team.Processors/Program.cs`

Expectation: no host-level logging pipeline rewiring is introduced by this feature.

## 2. Implement logging updates

1. Add constructor `ILogger<T>` to each in-scope Team component:
   - `team/apis/Team.Api/Services/*`
   - `team/apis/Team.Api/Services/Authorization/*`
   - `team/apis/Team.Api/Grpc/TeamGrpcService.cs`
   - `team/processors/Team.Processors/Subscribers/CustomerSubscriber.cs`
   - `team/shared/Team.Shared/Services/Cache/*`
   - `team/shared/Team.Shared/Publishers/TeamOutboxPublisher.cs`
   - `team/shared/Team.Shared/Activities/*`
   - `team/shared/Team.Shared/Services/TemporalOutboxService.cs`
2. Add structured logs according to policy:
   - Authorization denied: `LogWarning`
   - Authorization granted: `LogInformation`
   - Cache miss/eviction: `LogDebug`
   - Read methods: only denial/failure/empty-result logs
   - Mutation/publish/activity success: `LogInformation`
   - Exceptions caught and re-thrown: `LogError` before rethrow
3. Keep log properties secret-safe (IDs/counts/booleans/enums/outcomes only).

## 3. Update tests

1. Update unit tests affected by constructor signature changes.
2. Use `[AutoFakeItEasyData]` and `[Frozen]` patterns where required.
3. Avoid manual `NullLogger` and ad hoc logger construction.

## 4. Validate locally

Run focused Team tests first:

```bash
 dotnet test team/apis/Team.Api.UnitTests/Team.Api.UnitTests.csproj
 dotnet test team/processors/Team.Processors.UnitTests/Team.Processors.UnitTests.csproj
 dotnet test team/shared/Team.Shared.UnitTests/Team.Shared.UnitTests.csproj
```

Run integration tests for touched behaviours when required:

```bash
 dotnet test team/apis/Team.Api.IntegrationTests/Team.Api.IntegrationTests.csproj
 dotnet test team/processors/Team.Processors.IntegrationTests/Team.Processors.IntegrationTests.csproj
```

## 5. Review checklist before PR

- No `api-definitions/` or generated artefacts touched.
- Existing `LocationSubscriber` and `OrganizationSubscriber` behaviour unchanged.
- New logs contain no PII/secrets.
- Team tests pass.

## 6. Expected Validation Checkpoints

- **Checkpoint A**: Setup/foundational tasks complete and contract coverage checklist populated.
- **Checkpoint B**: US1 tests pass and mutation/auth logging policy confirmed.
- **Checkpoint C**: US2-US4 tests pass and shared component coverage confirmed.
- **Checkpoint D**: final focused test suite pass and safety review complete.

## 7. Latest Validation Run (2026-04-15)

- `dotnet test team/apis/Team.Api.UnitTests/Team.Api.UnitTests.csproj -v minimal`: passed (1/1)
- `dotnet test team/shared/Team.Shared.UnitTests/Team.Shared.UnitTests.csproj -v minimal`: passed (3/3)
- `dotnet test team/processors/Team.Processors.UnitTests/Team.Processors.UnitTests.csproj -v minimal`: run and confirm current processor unit tests pass
- `dotnet build` validation passed for:
  - `team/apis/Team.Api/Team.Api.csproj`
  - `team/shared/Team.Shared/Team.Shared.csproj`
  - `team/processors/Team.Processors/Team.Processors.csproj`
  - `team/jobs/Team.Jobs/Team.Jobs.csproj`
