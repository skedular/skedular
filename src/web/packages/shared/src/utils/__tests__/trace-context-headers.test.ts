import { describe, expect, it } from 'vitest';
import { getTraceContextHeaders } from '../trace-context-headers';

describe('getTraceContextHeaders', () => {
  it('forwards valid W3C trace context headers', () => {
    const headers = new Headers({
      traceparent: '00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01',
      tracestate: 'vendor=value',
      authorization: 'Bearer should-not-forward-here',
    });

    expect(getTraceContextHeaders(headers)).toEqual({
      traceparent: '00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01',
      tracestate: 'vendor=value',
    });
  });

  it('does not forward malformed or invalid traceparent headers', () => {
    expect(getTraceContextHeaders(new Headers({ traceparent: 'not-a-trace' }))).toEqual({});
    expect(getTraceContextHeaders(new Headers({ traceparent: '00-00000000000000000000000000000000-00f067aa0ba902b7-01' }))).toEqual({});
    expect(getTraceContextHeaders(new Headers({ traceparent: 'ff-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01' }))).toEqual({});
  });

  it('does not forward tracestate without a valid traceparent', () => {
    expect(getTraceContextHeaders(new Headers({ tracestate: 'vendor=value' }))).toEqual({});
  });
});
