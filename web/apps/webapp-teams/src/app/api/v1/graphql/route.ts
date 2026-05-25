import { authkit } from '@workos-inc/authkit-nextjs';
import { NextRequest, NextResponse } from 'next/server';
import { v7 as uuid } from 'uuid';
import { applyAuthkitResponseHeaders, getForwardedAuthorizationHeader } from '../authkit-response-headers';

const federatedGraphQLEndpoint = new URL('v1/graphql', process.env.GATEWAY_ENDPOINT).href;

const handler = async (request: NextRequest) => {
  const { headers: authkitHeaders, session } = await authkit(request);
  const authorization = request.headers.get('Authorization');
  const correlationId = request.headers.get('X-Correlation-Id') ?? uuid();
  const forwardedAuthorization = getForwardedAuthorizationHeader(authorization, session.accessToken);

  const headers: Record<string, string> = {
    'Content-Type': request.headers.get('Content-Type') ?? 'application/json',
    'X-Correlation-Id': correlationId,
    'X-SSO-Cookies': Buffer.from(JSON.stringify(request.cookies.getAll().filter((item) => item.name.startsWith('organization-sso'))), 'binary').toString('base64'),
  };

  if (forwardedAuthorization) {
    headers.Authorization = forwardedAuthorization;
  }

  const response = await fetch(federatedGraphQLEndpoint, {
    method: request.method,
    headers,
    body: await request.text(),
  });

  // Stream the response through so subscriptions (SSE) keep flowing.
  return applyAuthkitResponseHeaders(
    request,
    new NextResponse(response.body, {
      status: response.status,
      headers: response.headers,
    }),
    authkitHeaders,
  );
};

export { handler as GET, handler as POST };
