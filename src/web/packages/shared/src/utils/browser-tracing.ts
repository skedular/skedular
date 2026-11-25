import {ZoneContextManager} from '@opentelemetry/context-zone';
import {context, propagation, trace} from '@opentelemetry/api';
import {W3CTraceContextPropagator} from '@opentelemetry/core';
import {OTLPTraceExporter} from '@opentelemetry/exporter-trace-otlp-http';
import {registerInstrumentations} from '@opentelemetry/instrumentation';
import {FetchInstrumentation} from '@opentelemetry/instrumentation-fetch';
import {resourceFromAttributes} from '@opentelemetry/resources';
import {BatchSpanProcessor, TracerProvider} from '@opentelemetry/sdk-trace';

const installedKey = '__skedularBrowserTracingInstalled';

type BrowserTracingOptions = {
    serviceName: string;
    apiEndpoint: string;
    tracesEndpoint?: string;
};

const getApiOriginPattern = (apiEndpoint: string): RegExp => {
    const origin = new URL(apiEndpoint, window.location.href).origin;
    const escapedOrigin = origin.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    return new RegExp(`^${escapedOrigin}(?:/|$)`);
};

/** Starts browser HTTP spans and propagates W3C trace context to Skedular's API. */
export const installBrowserTracing = ({serviceName, apiEndpoint, tracesEndpoint}: BrowserTracingOptions): void => {
    if (typeof window === 'undefined') {
        return;
    }

    const browserGlobal = globalThis as typeof globalThis & { [installedKey]?: boolean };
    if (browserGlobal[installedKey]) {
        return;
    }

    const configuredTracesEndpoint = tracesEndpoint ?? (process.env.NODE_ENV === 'development' ? 'http://localhost:4318/v1/traces' : undefined);
    const spanProcessors = configuredTracesEndpoint ? [new BatchSpanProcessor({exporter: new OTLPTraceExporter({url: configuredTracesEndpoint})})] : [];
    const tracerProvider = new TracerProvider({
        spanProcessors,
        resource: resourceFromAttributes({'service.name': serviceName}),
    });

    context.setGlobalContextManager(new ZoneContextManager());
    propagation.setGlobalPropagator(new W3CTraceContextPropagator());
    trace.setGlobalTracerProvider(tracerProvider);

    registerInstrumentations({
        instrumentations: [
            new FetchInstrumentation({
                propagateTraceHeaderCorsUrls: [getApiOriginPattern(apiEndpoint)],
                ignoreUrls: configuredTracesEndpoint ? [configuredTracesEndpoint] : [],
            }),
        ],
        tracerProvider,
    });

    browserGlobal[installedKey] = true;
};
