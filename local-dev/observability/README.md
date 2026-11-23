# Local Observability Stack

The main Docker Compose stack includes Quickwit for logs and traces, Mimir for
metrics, Grafana, an OpenTelemetry Collector, and local S3-compatible object
storage. Quickwit uses the PostgreSQL service from the main Compose stack for
its metastore.

Start the full local stack from the repository root:

```bash
docker compose -p skedular -f docker-compose.yml up -d
```

## Endpoints

| Service          | Local endpoint                                                           |
|------------------|--------------------------------------------------------------------------|
| Grafana          | http://localhost:3000 (admin / admin)                                    |
| Quickwit API     | http://localhost:7280                                                    |
| Mimir            | http://localhost:9009/prometheus                                         |
| Jaeger | http://localhost:16686 |
| OTLP gRPC        | localhost:4317                                                           |
| OTLP HTTP        | http://localhost:4318                                                    |
| SeaweedFS S3 API | http://localhost:8333 (local-observability / local-observability-secret) |

Set `SKEDULAR_QUICKWIT_DB_PASSWORD` in the production `.env` file. The
same value is used for the PostgreSQL role and the Quickwit metastore URI.

Production domains:

| Service | Public endpoint |
|---|---|
| Grafana | https://grafana.skedular.app |
| Mimir | https://mimir.skedular.app |
| Quickwit | https://quickwit.skedular.app |

The collector accepts OTLP logs, traces, and metrics and exports logs and traces
to Quickwit and metrics to Mimir in the local tenant.

Quickwit applies a three-day retention policy to the `otel-logs-v0_9` index.
The `quickwit-log-retention-init` service creates or updates that policy when
the Compose stack starts.
