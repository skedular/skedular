import { getPublicOrigin } from '@skedular/shared';
import { authkit, handleAuthkitHeaders } from '@workos-inc/authkit-nextjs';
import { NextRequest, NextResponse } from 'next/server';

const getSessionCookieName = () => process.env.WORKOS_COOKIE_NAME || 'wos-session';

const isUnauthenticatedPath = (pathname: string) => {
  if (
    pathname === '/' ||
    pathname === '/callback' ||
    pathname === '/signin' ||
    pathname === '/signup' ||
    pathname === '/install-slack' ||
    pathname === '/slack-success-install' ||
    pathname === '/marketplace' ||
    pathname.startsWith('/auth/') ||
    pathname.startsWith('/msteams')
  ) {
    return true;
  }

  return pathname.startsWith('/marketplace/');
};

const handlePublicPathWithoutSession = (request: NextRequest, redirectUri: string) => {
  const requestHeaders = new Headers(request.headers);
  requestHeaders.set('x-workos-middleware', 'true');
  requestHeaders.set('x-url', request.url);
  requestHeaders.set('x-redirect-uri', redirectUri);

  return NextResponse.next({ request: { headers: requestHeaders } });
};

export default async function proxy(request: NextRequest) {
  const pathname = request.nextUrl.pathname;
  const redirectUri = new URL('/callback', getPublicOrigin(request)).toString();

  if (isUnauthenticatedPath(pathname) && !request.cookies.has(getSessionCookieName())) {
    return handlePublicPathWithoutSession(request, redirectUri);
  }

  const { headers, authorizationUrl, session } = await authkit(request, {
    debug: true,
    eagerAuth: true,
    redirectUri,
  });

  if (!session.user && authorizationUrl && !isUnauthenticatedPath(pathname)) {
    return handleAuthkitHeaders(request, headers, { redirect: authorizationUrl });
  }

  return handleAuthkitHeaders(request, headers);
}

export const config = {
  matcher: ['/((?!api|callback|signin|signup|_next/static|_next/image|favicon.ico|images|.*\\..*).*)'],
};
