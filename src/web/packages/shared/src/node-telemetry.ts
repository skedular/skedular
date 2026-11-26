import { logs } from '@opentelemetry/api-logs';
import { metrics, SpanKind } from '@opentelemetry/api';
import { OTLPLogExporter } from '@opentelemetry/exporter-logs-otlp-http';
import { OTLPMetricExporter } from '@opentelemetry/exporter-metrics-otlp-http';
import { resourceFromAttributes } from '@opentelemetry/resources';
import { LoggerProvider, BatchLogRecordProcessor } from '@opentelemetry/sdk-logs';
import { MeterProvider, PeriodicExportingMetricReader } from '@opentelemetry/sdk-metrics';

export const serverSpanKind = SpanKind.SERVER;

export const createPinoInstrumentation = async () => {
  const { PinoInstrumentation } = await import('@opentelemetry/instrumentation-pino');
  return new PinoInstrumentation();
};

type NodeTelemetryOptions = {
  serviceName: string;
  endpoint?: string;
};

/** Installs Node log/metric providers independently from the Vercel trace provider. */
export const installNodeTelemetry = ({ serviceName, endpoint }: NodeTelemetryOptions): (() => Promise<void>) => {
  if (!endpoint) return async () => undefined;

  const base = endpoint.replace(/\/$/, '');
  const resource = resourceFromAttributes({ 'service.name': serviceName });
  const loggerProvider = new LoggerProvider({
    processors: [new BatchLogRecordProcessor({ exporter: new OTLPLogExporter({ url: `${base}/v1/logs` }) })],
    resource,
  });
  const meterProvider = new MeterProvider({
    readers: [new PeriodicExportingMetricReader({ exporter: new OTLPMetricExporter({ url: `${base}/v1/metrics` }), exportIntervalMillis: 10000 })],
    resource,
  });

  logs.setGlobalLoggerProvider(loggerProvider);
  metrics.setGlobalMeterProvider(meterProvider);

  const meter = meterProvider.getMeter(`${serviceName}.process`);
  meter.createObservableGauge('nodejs_process_resident_memory_bytes', { unit: 'By' }).addCallback((observable) => {
    observable.observe(process.memoryUsage().rss);
  });
  meter.createObservableGauge('nodejs_process_heap_used_bytes', { unit: 'By' }).addCallback((observable) => {
    observable.observe(process.memoryUsage().heapUsed);
  });
  meter.createObservableGauge('nodejs_process_uptime_seconds', { unit: 's' }).addCallback((observable) => {
    observable.observe(process.uptime());
  });

  return () => Promise.all([meterProvider.forceFlush(), loggerProvider.forceFlush()]).then(() => undefined);
};
