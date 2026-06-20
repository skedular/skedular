import { authkit } from '@workos-inc/authkit-nextjs';
import { Buffer } from 'buffer';
import { NextRequest, NextResponse } from 'next/server';
import { applyAuthkitResponseHeaders, getForwardedAuthorizationHeader } from '../../../../authkit-response-headers';

const handler = async (request: NextRequest) => {
  const { headers: authkitHeaders, session } = await authkit(request);
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

  const headers: Record<string, string> = {
    'X-SSO-Cookies': Buffer.from(JSON.stringify(request.cookies.getAll().filter((item) => item.name.startsWith('organization-sso'))), 'binary').toString('base64'),
  };
  const forwardedAuthorization = getForwardedAuthorizationHeader(authorization, session.accessToken);
  if (forwardedAuthorization) {
    headers.Authorization = forwardedAuthorization;
  }

  const response = await fetch(targetUrl, {
    method: 'GET',
    headers,
    redirect: 'manual',
  });

  const location = response.headers.get('location');
  if (!location) {
    return applyAuthkitResponseHeaders(
      request,
      new NextResponse(response.body, {
        status: response.status,
        headers: response.headers,
      }),
      authkitHeaders,
    );
  }

  return applyAuthkitResponseHeaders(request, NextResponse.redirect(location, response.status), authkitHeaders);
};

export { handler as GET };
