import { authkitMiddleware } from '@workos-inc/authkit-nextjs';

export default authkitMiddleware();

export const config = {
  matcher: ['/', '/me', '/organizations', '/organizations/:slug*', '/notifications', '/notifications/:slug*'],
};
