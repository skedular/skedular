import { authkitMiddleware } from '@workos-inc/authkit-nextjs';

export default authkitMiddleware();

export const config = {
  matcher: ['/', '/organizations', '/organizations/:slug*', '/notifications', '/notifications/:slug*'],
};
