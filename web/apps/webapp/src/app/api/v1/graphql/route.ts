import { authkit } from '@workos-inc/authkit-nextjs';
import { NextRequest, NextResponse } from 'next/server';
import { v7 as uuid } from 'uuid';

const federatedGraphQLEndpoint = new URL('v1/graphql', process.env.GATEWAY_ENDPOINT).href;

const handler = async (request: NextRequest) => {
  const { session } = await authkit(request);
  const authorization = request.headers.get('Authorization');
  const correlationId = request.headers.get('X-Correlation-Id') ?? uuid();
  const accessToken = authorization ?? session.accessToken;

  const headers: Record<string, string> = {
    'Content-Type': request.headers.get('Content-Type') ?? 'application/json',
    'X-Correlation-Id': correlationId,
    'X-SSO-Cookies': Buffer.from(JSON.stringify(request.cookies.getAll().filter((item) => item.name.startsWith('organization-sso'))), 'binary').toString('base64'),
  };

  if (accessToken) {
    headers.Authorization = authorization ?? `Bearer ${accessToken}`;
  }

  const response = await fetch(federatedGraphQLEndpoint, {
    method: request.method,
    headers,
    body: await request.text(),
  });

  // Stream the response through so subscriptions (SSE) keep flowing.
  return new NextResponse(response.body, {
    status: response.status,
    headers: response.headers,
  });
};

export { handler as GET, handler as POST };
