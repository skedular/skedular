import { authkitMiddleware } from '@workos-inc/authkit-nextjs';

export default authkitMiddleware({
  debug: true,
  redirectUri: process.env.NEXT_PUBLIC_WORKOS_AUTHKIT_REDIRECT_URI!,
  middlewareAuth: {
    enabled: true,
    unauthenticatedPaths: ['/', '/marketplace', '/marketplace/:path*'],
  },
});

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
    '/organizations',
    '/organizations/:path*',
    '/notifications',
    '/notifications/:path*',
  ],
};
