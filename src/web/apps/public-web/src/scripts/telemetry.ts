import { context, propagation, trace } from "@opentelemetry/api";
import { W3CTraceContextPropagator } from "@opentelemetry/core";
import { OTLPMetricExporter } from "@opentelemetry/exporter-metrics-otlp-http";
import { OTLPTraceExporter } from "@opentelemetry/exporter-trace-otlp-http";
import { ZoneContextManager } from "@opentelemetry/context-zone";
import { registerInstrumentations } from "@opentelemetry/instrumentation";
import { FetchInstrumentation } from "@opentelemetry/instrumentation-fetch";
import { resourceFromAttributes } from "@opentelemetry/resources";
import {
  MeterProvider,
  PeriodicExportingMetricReader,
} from "@opentelemetry/sdk-metrics";
import { BatchSpanProcessor, TracerProvider } from "@opentelemetry/sdk-trace";
import { onCLS, onFCP, onINP, onLCP, onTTFB } from "web-vitals";

type Options = {
  serviceName?: string;
  apiEndpoint?: string;
  otlpEndpoint?: string;
};

let installed = false;

export function installPublicWebTelemetry(options: Options): void {
  if (
    installed ||
    !options.serviceName ||
    !options.apiEndpoint ||
    !options.otlpEndpoint
  ) {
    return;
  }

  installed = true;
  const base = options.otlpEndpoint.replace(/\/$/, "");
  const resource = resourceFromAttributes({
    "service.name": options.serviceName,
  });
  const tracerProvider = new TracerProvider({
    resource,
    spanProcessors: [
      new BatchSpanProcessor({
        exporter: new OTLPTraceExporter({ url: `${base}/v1/traces` }),
      }),
    ],
  });
  const meterProvider = new MeterProvider({
    resource,
    readers: [
      new PeriodicExportingMetricReader({
        exporter: new OTLPMetricExporter({ url: `${base}/v1/metrics` }),
        exportIntervalMillis: 10000,
      }),
    ],
  });

  context.setGlobalContextManager(new ZoneContextManager().enable());
  propagation.setGlobalPropagator(new W3CTraceContextPropagator());
  trace.setGlobalTracerProvider(tracerProvider);

  const apiOrigin = new URL(options.apiEndpoint, window.location.href).origin;
  registerInstrumentations({
    tracerProvider,
    instrumentations: [
      new FetchInstrumentation({
        propagateTraceHeaderCorsUrls: [
          new RegExp(
            `^${apiOrigin.replace(/[.*+?^${}()|[\]\\]/g, "\\$&")}(?:/|$)`,
          ),
        ],
        ignoreUrls: [`${base}/v1/traces`, `${base}/v1/metrics`],
      }),
    ],
  });

  const meter = meterProvider.getMeter("skedular.web-vitals");
  const histograms = {
    CLS: meter.createHistogram("web_vital_cls", { unit: "1" }),
    FCP: meter.createHistogram("web_vital_fcp", { unit: "ms" }),
    INP: meter.createHistogram("web_vital_inp", { unit: "ms" }),
    LCP: meter.createHistogram("web_vital_lcp", { unit: "ms" }),
    TTFB: meter.createHistogram("web_vital_ttfb", { unit: "ms" }),
  };
  const record = (metric: { name: keyof typeof histograms; value: number }) =>
    histograms[metric.name].record(metric.value);
  onCLS(record);
  onFCP(record);
  onINP(record);
  onLCP(record);
  onTTFB(record);

  document.addEventListener("visibilitychange", () => {
    if (document.visibilityState === "hidden") {
      void Promise.all([
        tracerProvider.forceFlush(),
        meterProvider.forceFlush(),
      ]).catch(() => undefined);
    }
  });
}
