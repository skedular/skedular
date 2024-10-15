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
    '/organizations',
    '/organizations/:slug*',
    '/locations',
    '/locations/:slug*',
    '/teams',
    '/teams/:slug*',
    '/notifications',
    '/notifications/:slug*',
    '/settings',
  ],
};
