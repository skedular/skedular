import { authkit } from '@workos-inc/authkit-nextjs';
import { Buffer } from 'buffer';
import { nanoid } from 'nanoid';
import { NextRequest, NextResponse } from 'next/server';

const federatedGraphQLEndpoint = new URL('v1/graphql', process.env.GATEWAY_ENDPOINT).href;

const handler = async (request: NextRequest) => {
  const { session } = await authkit(request);
  const authorization = request.headers.get('Authorization');

  const headers = {
    'Content-Type': request.headers.get('Content-Type') ?? 'application/json',
    'X-Correlation-Id': request.headers.get('X-Correlation-Id') ?? nanoid(),
    Authorization: authorization ? authorization : `Bearer ${session.accessToken}`,
    'X-SSO-Cookies': Buffer.from(JSON.stringify(request.cookies.getAll().filter((item) => item.name.startsWith('skedular-sso'))), 'binary').toString('base64'),
  };

  const response = await fetch(federatedGraphQLEndpoint, {
    method: request.method,
    headers,
    body: await request.text(),
  });

  return new NextResponse(await response.text());
};

export { handler as GET, handler as POST };
