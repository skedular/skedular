import { authkitMiddleware } from '@workos-inc/authkit-nextjs';

export default authkitMiddleware();

export const config = {
  matcher: [
    '/',
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
