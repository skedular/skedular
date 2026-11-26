import { context, propagation, trace } from '@opentelemetry/api';
import { logs, SeverityNumber } from '@opentelemetry/api-logs';
import { W3CTraceContextPropagator } from '@opentelemetry/core';
import { OTLPLogExporter } from '@opentelemetry/exporter-logs-otlp-http';
import { OTLPMetricExporter } from '@opentelemetry/exporter-metrics-otlp-http';
import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-http';
import { ZoneContextManager } from '@opentelemetry/context-zone';
import { registerInstrumentations } from '@opentelemetry/instrumentation';
import { FetchInstrumentation } from '@opentelemetry/instrumentation-fetch';
import { resourceFromAttributes } from '@opentelemetry/resources';
import { LoggerProvider, BatchLogRecordProcessor } from '@opentelemetry/sdk-logs';
import { MeterProvider, PeriodicExportingMetricReader } from '@opentelemetry/sdk-metrics';
import { BatchSpanProcessor, TracerProvider } from '@opentelemetry/sdk-trace';
import { onCLS, onFCP, onINP, onLCP, onTTFB } from 'web-vitals';

const installedKey = '__skedularBrowserTelemetryInstalled';
const sensitiveAttributePattern = /password|secret|token|authorization|cookie|email|phone|address/i;

type BrowserTelemetryOptions = {
  serviceName: string;
  apiEndpoint: string;
  otlpEndpoint?: string;
};

type PinoLogObject = {
  level?: number;
  msg?: string;
  time?: number;
  [key: string]: unknown;
};

const getApiRequestPatterns = (apiEndpoint: string): RegExp[] => {
  const origins = new Set([window.location.origin, new URL(apiEndpoint, window.location.href).origin]);
  return [...origins].map((origin) => {
    const escapedOrigin = origin.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    return new RegExp(`^${escapedOrigin}(?:/|$)`);
  });
};

const severityForPinoLevel = (level: number): SeverityNumber => {
  if (level >= 60) return SeverityNumber.FATAL;
  if (level >= 50) return SeverityNumber.ERROR;
  if (level >= 40) return SeverityNumber.WARN;
  if (level >= 30) return SeverityNumber.INFO;
  if (level >= 20) return SeverityNumber.DEBUG;
  return SeverityNumber.TRACE;
};

const installWebVitals = (meterProvider: MeterProvider): void => {
  const meter = meterProvider.getMeter('skedular.web-vitals');
  const histograms = {
    CLS: meter.createHistogram('web_vital_cls', { unit: '1' }),
    FCP: meter.createHistogram('web_vital_fcp', { unit: 'ms' }),
    INP: meter.createHistogram('web_vital_inp', { unit: 'ms' }),
    LCP: meter.createHistogram('web_vital_lcp', { unit: 'ms' }),
    TTFB: meter.createHistogram('web_vital_ttfb', { unit: 'ms' }),
  };

  const record = (metric: { name: keyof typeof histograms; value: number }): void => {
    histograms[metric.name].record(metric.value);
  };

  onCLS(record);
  onFCP(record);
  onINP(record);
  onLCP(record);
  onTTFB(record);
};

/** Starts browser traces, structured logs, and web-vitals metrics for a web app. */
export const installBrowserTelemetry = ({ serviceName, apiEndpoint, otlpEndpoint }: BrowserTelemetryOptions): void => {
  if (typeof window === 'undefined') return;

  const browserGlobal = globalThis as typeof globalThis & { [installedKey]?: boolean };
  if (browserGlobal[installedKey]) return;

  const endpoint = otlpEndpoint ?? (process.env.NODE_ENV === 'development' ? 'http://localhost:4318' : undefined);
  if (!endpoint) return;

  const resource = resourceFromAttributes({ 'service.name': serviceName });
  const tracesEndpoint = `${endpoint.replace(/\/$/, '')}/v1/traces`;
  const logsEndpoint = `${endpoint.replace(/\/$/, '')}/v1/logs`;
  const metricsEndpoint = `${endpoint.replace(/\/$/, '')}/v1/metrics`;

  const tracerProvider = new TracerProvider({
    spanProcessors: [new BatchSpanProcessor({ exporter: new OTLPTraceExporter({ url: tracesEndpoint }) })],
    resource,
  });
  const loggerProvider = new LoggerProvider({
    processors: [new BatchLogRecordProcessor({ exporter: new OTLPLogExporter({ url: logsEndpoint }) })],
    resource,
  });
  const meterProvider = new MeterProvider({
    readers: [new PeriodicExportingMetricReader({ exporter: new OTLPMetricExporter({ url: metricsEndpoint }), exportIntervalMillis: 10000 })],
    resource,
  });

  context.setGlobalContextManager(new ZoneContextManager().enable());
  propagation.setGlobalPropagator(new W3CTraceContextPropagator());
  trace.setGlobalTracerProvider(tracerProvider);
  logs.setGlobalLoggerProvider(loggerProvider);
  installWebVitals(meterProvider);
  document.addEventListener('visibilitychange', () => {
    if (document.visibilityState === 'hidden') {
      void meterProvider.forceFlush().catch(() => undefined);
    }
  });

  const browserLogger = loggerProvider.getLogger('skedular.browser');
  (globalThis as typeof globalThis & { __skedularBrowserLog?: (log: PinoLogObject) => void }).__skedularBrowserLog = (log) => {
    const attributes = Object.fromEntries(
      Object.entries(log).filter(
        ([key, value]) =>
          key !== 'level' &&
          key !== 'msg' &&
          key !== 'time' &&
          !sensitiveAttributePattern.test(key) &&
          (typeof value === 'string' || typeof value === 'number' || typeof value === 'boolean'),
      ),
    );
    browserLogger.emit({
      body: log.msg ?? '',
      severityNumber: severityForPinoLevel(log.level ?? 30),
      timestamp: log.time ? new Date(log.time) : new Date(),
      attributes,
    });
  };

  registerInstrumentations({
    instrumentations: [
      new FetchInstrumentation({
        // GraphQL requests use the same-origin Next.js proxy. Include that origin as
        // well as the configured API origin for direct API calls from the browser.
        propagateTraceHeaderCorsUrls: getApiRequestPatterns(apiEndpoint),
        ignoreUrls: [tracesEndpoint, logsEndpoint, metricsEndpoint],
      }),
    ],
    tracerProvider,
  });

  browserGlobal[installedKey] = true;
};
