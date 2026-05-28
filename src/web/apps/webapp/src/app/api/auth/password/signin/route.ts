import { getWorkOS } from '@workos-inc/authkit-nextjs';
import { NextRequest } from 'next/server';
import { authenticateAndRedirect, getSafeReturnTo, getWorkOSClientId, redirectToCustomAuth } from '../../../../auth/headless-workos';

export const POST = async (request: NextRequest) => {
  const formData = await request.formData();
  const email = formData.get('email');
  const password = formData.get('password');
  const returnTo = getSafeReturnTo(formData.get('returnTo'));

  if (typeof email !== 'string' || typeof password !== 'string' || !email.trim() || !password) {
    return redirectToCustomAuth(request, 'signin', 'missing_credentials', returnTo);
  }

  try {
    const authResponse = await getWorkOS().userManagement.authenticateWithPassword({
      clientId: getWorkOSClientId(),
      email: email.trim(),
      password,
      ipAddress: request.headers.get('x-forwarded-for')?.split(',')[0]?.trim(),
      userAgent: request.headers.get('user-agent') ?? undefined,
    });

    return authenticateAndRedirect(request, authResponse, returnTo);
  } catch {
    return redirectToCustomAuth(request, 'signin', 'invalid_credentials', returnTo);
  }
};
