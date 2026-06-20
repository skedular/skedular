import { getPublicOrigin } from '@skedular/shared';
import { authkit, handleAuthkitHeaders } from '@workos-inc/authkit-nextjs';
import { type NextRequest, NextResponse } from 'next/server';
import { isUnauthenticatedPath } from './libs/auth/route-access';

const getSessionCookieName = () => process.env.WORKOS_COOKIE_NAME || 'wos-session';

const continueWithoutSession = (request: NextRequest, redirectUri: string) => {
  const requestHeaders = new Headers(request.headers);
  requestHeaders.set('x-workos-middleware', 'true');
  requestHeaders.set('x-url', request.url);
  requestHeaders.set('x-redirect-uri', redirectUri);
  return NextResponse.next({ request: { headers: requestHeaders } });
};

export default async function proxy(request: NextRequest) {
  const redirectUri = new URL('/callback', getPublicOrigin(request)).toString();
  if (process.env.SKEDULAR_UI_TEST_BYPASS_AUTH === 'true' && !request.cookies.has(getSessionCookieName())) {
    return continueWithoutSession(request, redirectUri);
  }
  if (isUnauthenticatedPath(request.nextUrl.pathname) && !request.cookies.has(getSessionCookieName())) {
    return continueWithoutSession(request, redirectUri);
  }

  const { headers, authorizationUrl, session } = await authkit(request, { debug: true, eagerAuth: true, redirectUri });
  if (!session.user && authorizationUrl && !isUnauthenticatedPath(request.nextUrl.pathname)) {
    return handleAuthkitHeaders(request, headers, { redirect: authorizationUrl });
  }
  return handleAuthkitHeaders(request, headers);
}

export const config = {
  matcher: ['/((?!api|callback|signin|signup|_next/static|_next/image|favicon.ico|images|.*\\..*).*)'],
};
