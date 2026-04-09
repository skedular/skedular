# Skedular Shared Libraries — Architecture

This document covers every shared library under `shared/`.  It explains their
responsibilities, how they depend on each other, which domain projects reference them,
and what the internal structure of each library looks like.

Read it alongside `docs/architecture/scheduler-overview.md` for the cross-domain picture.

---

## 1. Library Dependency Diagram

Arrows point from **consumer → dependency**.

```mermaid
flowchart LR
    subgraph Domain["Domain projects (representative)"]
        BookingShared["Booking.Shared"]
        OrgShared["Organization.Shared"]
        LocShared["Location.Shared"]
        CustShared["Customer.Shared"]
        MktShared["Marketplace.Shared"]
        TeamShared["Team.Shared"]
        MsTeamsShared["MsTeams.Shared"]
        SlackShared["Slack.Shared"]
        CoreShared["Core.Shared"]
        GatewayShared["Gateway / YARP"]
    end

    subgraph Shared["Shared libraries"]
        ES["Enterprise.Shared"]
        ASC["Api.Shared.Clients"]
        ASS["Api.Shared.Services"]
        IS["Infrastructure.Shared"]
        TS["Testing.Shared"]
        TSI["Testing.Shared\n.IntegrationTests"]
    end

    BookingShared --> ES
    BookingShared --> ASC
    BookingShared --> ASS
    OrgShared --> ES
    OrgShared --> ASC
    OrgShared --> ASS
    LocShared --> ES
    LocShared --> ASC
    LocShared --> ASS
    CustShared --> ES
    CustShared --> ASC
    CustShared --> ASS
    MktShared --> ES
    MktShared --> ASC
    MktShared --> ASS
    TeamShared --> ES
    TeamShared --> ASC
    TeamShared --> ASS
    MsTeamsShared --> ES
    MsTeamsShared --> ASC
    MsTeamsShared --> ASS
    SlackShared --> ES
    SlackShared --> ASC
    SlackShared --> ASS
    CoreShared --> ES
    GatewayShared --> ASC

    IS --> ES

    TSI --> TS
    TSI --> ES

    ASC --> ES
    ASS --> ES
```

---

## 2. Enterprise.Shared — Component Map

`Enterprise.Shared` is the foundational cross-cutting library.  Every domain shared
library and every infrastructure project depends on it.

```mermaid
flowchart TB
    subgraph ES["Enterprise.Shared"]
        subgraph Messaging["Messaging"]
            K["Kafka\n• Produce / Consume helpers\n• Serialization\n• Health checks\n• KafkaClientNaming\n• KafkaPartitions\n• KafkaHelper\n• Telemetry\n• Logger\n• KafkaExitCodes"]
            T["Temporal\n• TemporalHelperService\n• Configurations\n• Extensions"]
            Out["Outbox\n• Kafka outbox dispatcher\n• Temporal outbox pattern"]
        end

        subgraph Persistence["Persistence"]
            DB["Database\n• EF Core EntityBase\n• Interceptors\n• IRepository / IUnitOfWork\n• Specification evaluator\n• DbTransactionBuilder\n• Postgres / SqlServer helpers\n• DatabaseMigrationService"]
            Cache["Cache\n• Redis helpers\n• Extensions"]
        end

        subgraph Web["Web / API"]
            GQL["GraphQL\n• HotChocolate helpers\n• Extensions"]
            HTTP["Http\n• HttpClient helpers"]
            GRPC["Grpc\n• gRPC helpers"]
            Pag["Pagination\n• Cursor / offset helpers"]
        end

        subgraph Auth["Auth / Identity"]
            Sec["Security\n• SSO / JWT helpers\n• WorkOS integration\n• Token helpers\n• CookieEncryptionService\n• StringEncryptionAlgorithm\n• ICustomerHelper"]
        end

        subgraph Integrations["External integrations"]
            Pay["Payment\n• Stripe helpers"]
            Acc["Accounting\n• Xero token encryption\n• IXeroTokenEncryptionService\n• IXeroSdkClientFactory registration"]
            Az["Azure\n• Azure Active Directory\n• Graph API helpers"]
            AI["Ai\n• AI provider helpers"]
        end

        subgraph Infra["Infrastructure / Cross-cutting"]
            Tel["Telemetry\n• OpenTelemetry setup"]
            Log["Logging\n• Structured logging helpers"]
            Met["Metrics\n• Custom metric helpers"]
            Email["Email\n• Email sending helpers"]
            FS["FileStorage\n• Blob / CDN helpers"]
            Time["Time\n• ITimeProvider abstraction"]
            Rnd["Random\n• IRandom abstraction"]
            San["Sanitization\n• Input sanitizers"]
            Mdl["Models\n• Shared value objects"]
        end
    end
```

---

## 3. Api.Shared.Clients — Layout

Generated HTTP clients, typed Kafka event definitions, and gRPC client stubs used by all
consumers of the domain APIs.

```mermaid
flowchart LR
    subgraph ASC["Api.Shared.Clients"]
        subgraph Events["Events/Skedular — Kafka topic definitions"]
            EBK["Booking\n• BookingTopic\n• event metadata companions"]
            EBI["BookingInternal\n• BookingInternalTopic"]
            ECU["Customer\n• CustomerTopic"]
            ELO["Location\n• LocationTopic"]
            EMK["Marketplace\n• MarketplaceTopic"]
            EOR["Organization\n• OrganizationTopic"]
            EOI["OrganizationInternal\n• OrganizationInternalTopic"]
            EOM["OrganizationMember\n• OrganizationMemberTopic"]
            ETM["Team\n• TeamTopic"]
        end

        subgraph OpenApi["OpenApi/Skedular — generated HTTP clients"]
            OBK["Booking client"]
            OCO["Core client"]
            OCU["Customer client"]
            OGW["Gateway client"]
            OLO["Location client"]
            OMK["Marketplace client"]
            OMS["MsTeams client"]
            OOR["Organization client"]
            OSL["Slack client"]
            OTM["Team client"]
        end

        subgraph Grpc["Grpc — generated gRPC stubs"]
            GC["GrpcClients.cs\n• Core gRPC stub\n• (identity/customer lookup)"]
        end

        Cfg["Configurations\n• client registration helpers"]
    end
```

---

## 4. Api.Shared.Services — Layout

Generated OpenAPI controller base classes and shared service models used by all domain
API projects.

```mermaid
flowchart LR
    subgraph ASS["Api.Shared.Services"]
        subgraph OpenApi["OpenApi/Skedular — generated controller bases"]
            SBK["Booking controller base"]
            SCO["Core controller base"]
            SCU["Customer controller base"]
            SGW["Gateway controller base"]
            SLO["Location controller base"]
            SMK["Marketplace controller base"]
            SMS["MsTeams controller base"]
            SOR["Organization controller base"]
            SSL["Slack controller base"]
            STM["Team controller base"]
        end

        subgraph Offering["Offering"]
            OF["Offerings.cs — offering definitions"]
            FF["Features.cs — feature flags"]
        end

        subgraph Models["Models — shared request/response models"]
            direction TB
            M1["AddressDetails, ContactDetails"]
            M2["BookingCategory, BookingChannel\nBookingSchedules"]
            M3["ProductPricing, ProductLineItem\nProductType, PriceUnit"]
            M4["OrganizationMemberRole/Status\nOrganizationType, OrganizationBillingCycle"]
            M5["CustomerType, CustomerPersonalDetails\nPersonalInformationVisibility"]
            M6["PaymentStatus, PaymentMethod\nMarketplaceBookingSubscriptionStatus"]
            M7["IdentityDetails, CdnImageFile\nOpeningHours, PeopleCapacity"]
            M8["TeamMemberRole/Status\nInvitationStatus, DayOfWeek\nCurrency, AreaRange, Polygon"]
        end

        PE["PriceExtensions.cs\n• price calculation helpers"]
        EX["Extensions.cs / Constants.cs\n• shared helpers"]
    end
```

---

## 5. Infrastructure.Shared — Migration Host Pattern

`Infrastructure.Shared` is used as the database-migration host for every domain's schema.
Each domain runs it as a short-lived job that applies EF Core migrations and exits.

```mermaid
flowchart TB
    subgraph IS["Infrastructure.Shared"]
        Prog["Program.cs\n• host builder entry point"]
        MS["MigrationService\n• orchestrates migration run"]
        MJ["InfrastructureMigrationJob\n• IHostedService implementation"]
        Cfg["appsettings.json\n• connection string config"]
    end

    subgraph DomainMigration["Per-domain migration project"]
        DM["Domain.Infrastructure\n• EF Core DbContext\n• Migrations/ folder\n• references Infrastructure.Shared"]
    end

    subgraph Runtime["Runtime"]
        K8sJob["Kubernetes Job /\nDocker Compose service\n(short-lived)"]
        PG["PostgreSQL\n(domain schema)"]
    end

    K8sJob --> Prog
    Prog --> MJ
    MJ --> MS
    MS --> DM
    DM --> PG
```

---

## 6. Testing.Shared and Testing.Shared.IntegrationTests

These libraries provide test infrastructure shared across all unit and integration test
projects.

```mermaid
flowchart LR
    subgraph TS["Testing.Shared"]
        direction TB
        AF["AutoFakeItEasyDataAttribute\nInlineAutoFakeItEasyDataAttribute\n• xUnit + AutoFixture + FakeItEasy\n  data-driven test attributes"]
        CL["ConsoleLogger\n• ILogger backed by Console\n  for test output"]
        Gen["Generators\n• CoordinateGenerator\n• DateTimeOffsetGenerator\n• CancellationTokenGenerator\n• TimeProviderGenerator\n• RandomGenerators\n• ExceptionContextGenerator"]
        Ass["Assertions\n• fluent assertion helpers"]
        Cat["CategoryNames.cs\n• xUnit trait category constants"]
    end

    subgraph TSI["Testing.Shared.IntegrationTests"]
        direction TB
        Asp["Aspire/\n• DistributedApplication\n  test app host helpers\n• WaitFor / WaitForCompletion\n  readiness extensions"]
        Ev["Eventually.cs\n• retry-until-true helper\n  for eventual consistency assertions"]
        GF["GrpcChannelFactory.cs\n• creates gRPC test channels"]
        HE["HttpClientExtensions.cs\n• typed HTTP helpers for\n  integration test clients"]
        Ex["Extensions.cs\n• shared integration test\n  setup extensions"]
    end

    subgraph Consumers["Test projects that use them"]
        UT["*.UnitTests projects\n(all domains)"]
        IT["*.IntegrationTests projects\n(all domains)"]
    end

    UT --> TS
    IT --> TS
    IT --> TSI
```

---

## 7. Directory Reference

| Path | Library | Role |
|---|---|---|
| `shared/Enterprise.Shared/` | Enterprise.Shared | Foundational cross-cutting library |
| `shared/Enterprise.Shared/Kafka/` | Enterprise.Shared | Kafka produce/consume helpers |
| `shared/Enterprise.Shared/Temporal/` | Enterprise.Shared | Temporal configuration + helpers |
| `shared/Enterprise.Shared/Database/` | Enterprise.Shared | EF Core base entities, repositories, interceptors |
| `shared/Enterprise.Shared/Security/` | Enterprise.Shared | SSO, JWT, WorkOS, encryption |
| `shared/Enterprise.Shared/Cache/` | Enterprise.Shared | Redis helpers |
| `shared/Enterprise.Shared/GraphQL/` | Enterprise.Shared | HotChocolate helpers |
| `shared/Enterprise.Shared/Payment/` | Enterprise.Shared | Stripe helpers |
| `shared/Enterprise.Shared/Accounting/` | Enterprise.Shared | Xero token encryption, `AddXeroServices` |
| `shared/Enterprise.Shared/Azure/` | Enterprise.Shared | Azure Entra / Graph API helpers |
| `shared/Enterprise.Shared/Outbox/` | Enterprise.Shared | Kafka + Temporal outbox patterns |
| `shared/Enterprise.Shared/Telemetry/` | Enterprise.Shared | OpenTelemetry setup |
| `shared/Enterprise.Shared/Ai/` | Enterprise.Shared | AI provider helpers |
| `shared/Api.Shared.Clients/` | Api.Shared.Clients | Generated OpenAPI clients + event topics + gRPC stubs |
| `shared/Api.Shared.Clients/Events/Skedular/` | Api.Shared.Clients | Typed Kafka topic definitions (9 topics) |
| `shared/Api.Shared.Clients/OpenApi/Skedular/` | Api.Shared.Clients | Generated HTTP API clients (10 domains) |
| `shared/Api.Shared.Clients/Grpc/` | Api.Shared.Clients | Generated gRPC client stubs (Core) |
| `shared/Api.Shared.Services/` | Api.Shared.Services | Generated OpenAPI controller bases + shared models |
| `shared/Api.Shared.Services/OpenApi/Skedular/` | Api.Shared.Services | Generated controller base classes (10 domains) |
| `shared/Api.Shared.Services/Offering/` | Api.Shared.Services | Offering definitions + feature flags |
| `shared/Api.Shared.Services/Models/` | Api.Shared.Services | Shared request/response value objects |
| `shared/Infrastructure.Shared/` | Infrastructure.Shared | EF Core migration host (MigrationService + job) |
| `shared/Testing.Shared/` | Testing.Shared | xUnit unit test helpers (AutoFixture, generators, assertions) |
| `shared/Testing.Shared.IntegrationTests/` | Testing.Shared.IntegrationTests | Aspire integration test base, Eventually helper, gRPC/HTTP helpers |
