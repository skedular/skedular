import { getWorkOS } from '@workos-inc/authkit-nextjs';
import { NextRequest } from 'next/server';
import { authenticateAndRedirect, getSafeReturnTo, getWorkOSClientId, redirectToCustomAuth } from '../../../../auth/headless-workos';

export const POST = async (request: NextRequest) => {
  const formData = await request.formData();
  const email = formData.get('email');
  const password = formData.get('password');
  const confirmPassword = formData.get('confirmPassword');
  const returnTo = getSafeReturnTo(formData.get('returnTo'));

  if (typeof email !== 'string' || typeof password !== 'string' || !email.trim() || !password) {
    return redirectToCustomAuth(request, 'signup', 'missing_credentials', returnTo);
  }

  if (password !== confirmPassword) {
    return redirectToCustomAuth(request, 'signup', 'password_mismatch', returnTo);
  }

  try {
    const workos = getWorkOS();
    await workos.userManagement.createUser({
      email: email.trim(),
      password,
    });

    const authResponse = await workos.userManagement.authenticateWithPassword({
      clientId: getWorkOSClientId(),
      email: email.trim(),
      password,
      ipAddress: request.headers.get('x-forwarded-for')?.split(',')[0]?.trim(),
      userAgent: request.headers.get('user-agent') ?? undefined,
    });

    return authenticateAndRedirect(request, authResponse, returnTo);
  } catch {
    return redirectToCustomAuth(request, 'signup', 'create_account_failed', returnTo);
  }
};
