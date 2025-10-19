import { authkitMiddleware } from '@workos-inc/authkit-nextjs';

export default authkitMiddleware({
  middlewareAuth: {
    enabled: true,
    unauthenticatedPaths: ['/'],
  },
});

export const config = {
  matcher: ['/welcome', '/bookings', '/notifications', '/billing-and-payment', '/settings', '/organizations', '/organizations/:path*', '/notifications', '/notifications/:path*'],
};
