const W3C_TRACEPARENT_PATTERN = /^([0-9a-f]{2})-([0-9a-f]{32})-([0-9a-f]{16})-([0-9a-f]{2})$/i;

/** Returns only valid W3C trace context headers for forwarding across a trusted proxy. */
export const getTraceContextHeaders = (headers: Headers): Record<string, string> => {
  const traceparent = headers.get('traceparent');
  if (!traceparent || !W3C_TRACEPARENT_PATTERN.test(traceparent)) {
    return {};
  }

  const [, version, traceId, parentId] = traceparent.match(W3C_TRACEPARENT_PATTERN) ?? [];
  if (version === 'ff' || /^0+$/.test(traceId ?? '') || /^0+$/.test(parentId ?? '')) return {};

  const forwarded: Record<string, string> = { traceparent };
  const tracestate = headers.get('tracestate');
  if (tracestate) {
    forwarded.tracestate = tracestate;
  }

  return forwarded;
};
