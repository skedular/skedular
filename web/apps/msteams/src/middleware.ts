import { withAuth } from 'next-auth/middleware';

export default withAuth({
  callbacks: {
    authorized({ req, token }) {
      return !!token;
    },
  },
});

export const config = {
  matcher: [
    '/',
    '/organization',
    '/organization/:slug*',
    '/notification',
    '/notification/:slug*',
    '/location',
    '/location/:slug*',
    '/team',
    '/team/:slug*',
    '/settings',
  ],
};
