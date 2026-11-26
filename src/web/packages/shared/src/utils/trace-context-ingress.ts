import { context, trace } from '@opentelemetry/api';
import { getTraceContextHeaders } from './trace-context-headers';

type TraceContextIngressRequest = {
  headers: Headers;
  method: string;
  url: string;
};

/** Logs only trace identifiers and request routing metadata for a server-side web app ingress. */
export const logTraceContextIngress = (serviceName: string, request: TraceContextIngressRequest): void => {
  const traceparent = getTraceContextHeaders(request.headers).traceparent;
  const traceparentParts = traceparent?.split('-');
  const incomingTraceId = traceparentParts?.[1];
  const incomingParentSpanId = traceparentParts?.[2];
  const activeSpanContext = trace.getSpan(context.active())?.spanContext();

  console.info(
    JSON.stringify({
      event: 'http.trace_context.ingress',
      service_name: serviceName,
      http_request_method: request.method,
      http_route: new URL(request.url).pathname,
      traceparent_present: Boolean(request.headers.get('traceparent')),
      traceparent_valid: Boolean(traceparent),
      incoming_trace_id: incomingTraceId ?? null,
      incoming_parent_span_id: incomingParentSpanId ?? null,
      active_trace_id: activeSpanContext?.traceId ?? null,
      active_span_id: activeSpanContext?.spanId ?? null,
      active_trace_matches_incoming: activeSpanContext?.traceId === incomingTraceId,
    }),
  );
};
