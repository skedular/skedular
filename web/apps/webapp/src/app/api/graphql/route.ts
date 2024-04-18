import { v4 as uuidv4 } from 'uuid';
import { getToken } from 'next-auth/jwt';
import { NextRequest, NextResponse } from 'next/server';

const federatedGraphQLEndpoint = `${process.env.GATEWAY_ENDPOINT}/gateway/api/v1/graphql`;

const handler = async (request: NextRequest) => {
  const token = await getToken({ req: request });
  if (!token || !token.idToken) {
    return NextResponse.json({}, { status: 401 });
  }

  const headers = {
    'Content-Type': request.headers.get('Content-Type') ?? 'application/json',
    'X-Correlation-Id': request.headers.get('X-Correlation-Id') ?? uuidv4(),
    Authorization: `Bearer ${token.idToken}`,
  };

  const response = await fetch(federatedGraphQLEndpoint, {
    method: request.method,
    headers,
    body: await request.text(),
  });

  return new NextResponse(await response.text());
};

export { handler as GET, handler as POST };
