import serverLogger from '@/libs/logging';
import { authkit } from '@workos-inc/authkit-nextjs';
import { Buffer } from 'buffer';
import { NextRequest, NextResponse } from 'next/server';
import { v7 as uuid } from 'uuid';

const federatedGraphQLEndpoint = new URL('v1/graphql', process.env.GATEWAY_ENDPOINT).href;
const gatewayFetchTimeoutMs = Number(process.env.GATEWAY_FETCH_TIMEOUT_MS ?? 60000);

const handler = async (request: NextRequest) => {
  const startedAt = Date.now();
  const { session } = await authkit(request);
  const authorization = request.headers.get('Authorization');
  const correlationId = request.headers.get('X-Correlation-Id') ?? uuid();

  const headers = {
    'Content-Type': request.headers.get('Content-Type') ?? 'application/json',
    'X-Correlation-Id': correlationId,
    Authorization: authorization ? authorization : `Bearer ${session.accessToken}`,
    'X-SSO-Cookies': Buffer.from(JSON.stringify(request.cookies.getAll().filter((item) => item.name.startsWith('organization-sso'))), 'binary').toString('base64'),
  };

  serverLogger.info(
    {
      event: 'graphql_proxy_request_start',
      correlationId,
      method: request.method,
      path: request.nextUrl.pathname,
      gatewayUrl: federatedGraphQLEndpoint,
      authorizationHeader: headers.Authorization,
    },
    'GraphQL proxy request started',
  );

  const abortController = new AbortController();
  let abortReason: 'request_aborted' | 'timeout' | null = null;
  const onRequestAbort = () => abortController.abort();

  request.signal.addEventListener('abort', onRequestAbort, { once: true });

  const timeoutId = setTimeout(() => {
    abortReason = 'timeout';
    abortController.abort();
  }, gatewayFetchTimeoutMs);

  const onAbort = () => {
    if (request.signal.aborted) {
      abortReason = 'request_aborted';
    }
  };

  abortController.signal.addEventListener('abort', onAbort, { once: true });
  let response: Response;

  try {
    response = await fetch(federatedGraphQLEndpoint, {
      method: request.method,
      headers,
      body: await request.text(),
      signal: abortController.signal,
    });

    serverLogger.info(
      {
        event: 'graphql_proxy_request_end',
        correlationId,
        method: request.method,
        path: request.nextUrl.pathname,
        statusCode: response.status,
        elapsedMs: Date.now() - startedAt,
      },
      'GraphQL proxy request completed',
    );
  } catch (error) {
    serverLogger.error(
      {
        event: 'graphql_proxy_request_failed',
        correlationId,
        method: request.method,
        path: request.nextUrl.pathname,
        elapsedMs: Date.now() - startedAt,
        timeoutMs: gatewayFetchTimeoutMs,
        requestAborted: request.signal.aborted,
        abortReason,
        errorName: error instanceof Error ? error.name : 'UnknownError',
      },
      'GraphQL proxy request failed',
    );

    throw error;
  } finally {
    clearTimeout(timeoutId);
    request.signal.removeEventListener('abort', onRequestAbort);
    abortController.signal.removeEventListener('abort', onAbort);
  }

  // Stream the response through so subscriptions (SSE) keep flowing.
  return new NextResponse(response.body, {
    status: response.status,
    headers: response.headers,
  });
};

export { handler as GET, handler as POST };
