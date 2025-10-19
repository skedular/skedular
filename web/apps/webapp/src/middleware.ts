import { authkitMiddleware } from '@workos-inc/authkit-nextjs';

export default authkitMiddleware({
  debug: true,
  redirectUri: process.env.NEXT_PUBLIC_WORKOS_AUTHKIT_REDIRECT_URI!,
  middlewareAuth: {
    enabled: true,
    unauthenticatedPaths: ['/'],
  },
});

export const config = {
  matcher: ['/welcome', '/bookings', '/notifications', '/billing-and-payment', '/settings', '/organizations', '/organizations/:path*', '/notifications', '/notifications/:path*'],
};
