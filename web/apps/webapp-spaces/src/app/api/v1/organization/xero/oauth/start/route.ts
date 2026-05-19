import { authkit } from '@workos-inc/authkit-nextjs';
import { Buffer } from 'buffer';
import { NextRequest, NextResponse } from 'next/server';

const handler = async (request: NextRequest) => {
  const { session } = await authkit(request);
  const authorization = request.headers.get('Authorization');
  const searchParams = request.nextUrl.searchParams;
  const targetUrl = new URL('/v1/organization/xero/oauth/start', process.env.GATEWAY_ENDPOINT);

  const organizationId = searchParams.get('organizationId');
  const organizationCustomDomain = searchParams.get('organizationCustomDomain');
  if (organizationId) {
    targetUrl.searchParams.set('organizationId', organizationId);
  }

  if (organizationCustomDomain) {
    targetUrl.searchParams.set('organizationCustomDomain', organizationCustomDomain);
  }

  const response = await fetch(targetUrl, {
    method: 'GET',
    headers: {
      Authorization: authorization ? authorization : `Bearer ${session.accessToken}`,
      'X-SSO-Cookies': Buffer.from(JSON.stringify(request.cookies.getAll().filter((item) => item.name.startsWith('organization-sso'))), 'binary').toString('base64'),
    },
    redirect: 'manual',
  });

  const location = response.headers.get('location');
  if (!location) {
    return new NextResponse(response.body, {
      status: response.status,
      headers: response.headers,
    });
  }

  return NextResponse.redirect(location, response.status);
};

export { handler as GET };
