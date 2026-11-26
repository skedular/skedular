import {OTLPHttpProtoTraceExporter, registerOTel} from '@vercel/otel'
import {after} from 'next/server'

const serviceName = process.env.OTEL_SERVICE_NAME ?? 'webapp-host'
const endpoint = (process.env.OTEL_EXPORTER_OTLP_ENDPOINT ?? (process.env.NODE_ENV === 'development' ? 'http://localhost:4318' : undefined))?.replace(/\/$/, '')
const traceExporter = endpoint ? new OTLPHttpProtoTraceExporter({url: `${endpoint}/v1/traces`}) : undefined
export async function register() {
  if (process.env.NEXT_RUNTIME === 'nodejs') {
    const {createPinoInstrumentation, installNodeTelemetry, serverSpanKind} = await import('@skedular/shared/node-telemetry')
    const flushMetrics = installNodeTelemetry({serviceName, endpoint})
    const metricFlushProcessor = {
      forceFlush: flushMetrics,
      onStart() { },
      onEnd(span: {kind: number}) {
        if (span.kind !== serverSpanKind) return
        try {
          after(() => flushMetrics().catch(() => undefined))
        } catch {
          // Server spans outside a Next.js request scope use the periodic reader.
        }
      },
      shutdown: async () => undefined,
    }

    registerOTel({
      serviceName,
      instrumentations: ['fetch', await createPinoInstrumentation()],
      // Use the configured OTLP destination directly. Vercel's automatic
      // exporter can prefer its own runtime collector over OTEL_EXPORTER_OTLP_ENDPOINT.
      spanProcessors: traceExporter ? [metricFlushProcessor] : ['auto', metricFlushProcessor],
      ...(traceExporter ? {traceExporter} : {}),
    })
  } else {
    registerOTel({serviceName, ...(traceExporter ? {spanProcessors: [], traceExporter} : {})})
  }
}
