import { authkitMiddleware } from '@workos-inc/authkit-nextjs';

export default authkitMiddleware({ debug: true });

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
