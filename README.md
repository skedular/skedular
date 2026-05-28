# Skedular

Skedular Mono Repository

## Local Development Environment

### Prerequisites

- Docker and Docker Compose
- .NET 10.0 SDK
- Node.js 22.x and pnpm
- Make (for running Makefile commands)

### Getting Started

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd unityhubio
   ```

2. **Create environment file**

   ```bash
   cp .env.template .env
   # Edit .env file with your configuration
   ```

3. **Start infrastructure dependencies**

   ```bash
   # Using the provided script
   ./scripts/start-dependencies.sh

   # Or using docker-compose directly
   docker compose -p skedular -f docker-compose.yml --env-file .env up -d
   ```

   This starts:

    - PostgreSQL database (port 5432)
    - Redis cache (port 6379)
    - Kafka message broker (port 9092)
    - Keycloak authentication
    - Temporal workflow engine
    - Dozzle for log viewing (port 5001)

4. **Generate GraphQL schemas and federation gateway**

   ```bash
   # This step is important for GraphQL federation to work properly
   # It exports schemas from each service and composes them into the gateway
   ./scripts/generate-graphql.sh

   # Or use the make command which also generates other API definitions
   make generate
   ```

5. **Run database migrations**

   ```bash
   # Run all migrations at once
   dotnet run --project src/all-in-one/AllInfra

   # Or run individual service migrations if needed
   # dotnet run --project src/location/shared/Location.Infrastructure
   # dotnet run --project src/organization/shared/Organization.Infrastructure
   # ... etc
   ```

6. **Start backend services**

   ```bash
   # Option 1: Run all APIs and Gateway at once (recommended)
   dotnet run --project src/all-in-one/AllApis

   # Option 2: Run everything (APIs, Processors, Jobs, and Gateway)
   # Note: This also runs migrations first
   dotnet run --project src/all-in-one/AllInOne

   # Option 3: Run services individually (for debugging specific services)
   # dotnet run --project src/gateway/apis/Gateway
   # dotnet run --project src/organization/apis/Organization.Api
   # dotnet run --project src/location/apis/Location.Api
   # ... etc
   ```

7. **Start frontend application**
   ```bash
   cd web
   pnpm install
   pnpm dev
   ```

### Service URLs

- **Frontend**: http://localhost:15000
- **Gateway GraphQL**: http://localhost:9000/v1/graphql
- **Dozzle (logs)**: http://localhost:5001
- **Organization API**: http://localhost:10200
- **Booking API**: http://localhost:10300
- **Customer API**: http://localhost:10000
- **Location API**: http://localhost:10600
- **Team API**: http://localhost:10500
- **Marketplace API**: http://localhost:11000
- **Core API**: http://localhost:11100

### Common Development Tasks

#### Code formatting

```bash
# Format .NET code using JetBrains CleanupCode
./scripts/format.sh

# Or use make (also formats Terraform and Go files)
make format
```

#### Run linting

```bash
# Lint .NET code using JetBrains InspectCode
./scripts/lint.sh

# Or use make (also checks Terraform formatting)
make lint
```

#### Regenerate GraphQL schemas and code

```bash
# Generate GraphQL schemas and compose federation gateway
./scripts/generate-graphql.sh

# Or use make (also generates API definitions and frontend types)
make generate
```

#### View logs

```bash
# Using Dozzle web interface
open http://localhost:5001

# Or using docker logs
docker logs -f skedular-postgres-1
docker logs -f skedular-redis-1
```

### Using Docker Compose for All Services

For a fully containerized environment:

```bash
# Start all services (production mode)
make services-all-start

# Stop all services
make services-all-stop

# Restart all services
make services-all-restart

# Terminate all services and volumes
make services-all-terminate
```

### Troubleshooting

#### Database Connection Issues

- Ensure PostgreSQL is running: `docker ps | grep postgres`
- Check credentials in `.env` file match docker-compose.yml
- Default connection: `postgres://8b974997c3c54b10a556f089377505d7:123456@localhost:5432/skedular`

#### Port Conflicts

If services fail to start due to port conflicts:

```bash
# Find process using a port
lsof -i :PORT

# Kill process
kill -9 PID
```

#### GraphQL Errors

For "Cannot return null for non-nullable field" errors:

1. Check for null data in database tables
2. Run database cleanup if needed:
   ```sql
   -- Example: Clear floor plan data
   DELETE FROM "ResourcePositions";
   DELETE FROM "FloorPlans";
   ```

#### Reset Development Environment

```bash
# Stop all services
./scripts/start-dependencies.sh down

# Remove volumes (WARNING: deletes all data)
docker volume prune

or

./scripts/start-dependencies.sh down -v


# Restart
./scripts/start-dependencies.sh
```

## [Codebase Overview](docs/codebase-overview.md)

## Architecture Decision Records

- [View all ADRs](docs/adr-index.md)

## [Single Sign-On (SSO) Integration](docs/sso-integration.md)

# Stripe forward command

```shell
stripe listen -l --forward-to http://0.0.0.0:9000/v1/organization/stripe/platform/account/webhook --forward-connect-to http://0.0.0.0:9000/v1/organization/stripe/connect/account/webhook
stripe listen -l --forward-to http://0.0.0.0:9000/v1/booking/stripe/platform/account/webhook --forward-connect-to http://0.0.0.0:9000/v1/booking/stripe/connect/account/webhook
```
