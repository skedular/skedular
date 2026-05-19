import { getPublicOrigin } from '@skedular/shared';
import { authkit, handleAuthkitHeaders } from '@workos-inc/authkit-nextjs';
import type { NextRequest } from 'next/server';

const isUnauthenticatedPath = (pathname: string) => {
  if (pathname === '/' || pathname === '/marketplace' || pathname.startsWith('/auth/')) {
    return true;
  }

  return pathname.startsWith('/marketplace/');
};

export default async function proxy(request: NextRequest) {
  const redirectUri = new URL('/callback', getPublicOrigin(request)).toString();
  const { headers, authorizationUrl, session } = await authkit(request, {
    debug: true,
    redirectUri,
  });

  if (!session.user && authorizationUrl && !isUnauthenticatedPath(request.nextUrl.pathname)) {
    return handleAuthkitHeaders(request, headers, { redirect: authorizationUrl });
  }

  return handleAuthkitHeaders(request, headers);
}

export const config = {
  matcher: [
    '/',
    '/welcome',
    '/bookings',
    '/notifications',
    '/billing-and-payment',
    '/settings',
    '/marketplace',
    '/marketplace/:path*',
    '/auth/:path*',
    '/organizations',
    '/organizations/:path*',
    '/notifications',
    '/notifications/:path*',
  ],
};
