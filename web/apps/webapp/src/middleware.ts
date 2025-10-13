import { authkitMiddleware } from '@workos-inc/authkit-nextjs';

export default authkitMiddleware({ debug: true, redirectUri: process.env.NEXT_PUBLIC_WORKOS_REDIRECT_URI! });

export const config = {
  matcher: [
    '/:slug*',
    '/welcome',
    '/bookings',
    '/notifications',
    '/billing-and-payment',
    '/settings',
    '/organizations',
    '/organizations/:slug*',
    '/notifications',
    '/notifications/:slug*',
  ],
};
