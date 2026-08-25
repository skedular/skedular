import { getWorkOS } from '@workos-inc/authkit-nextjs';
import { NextRequest } from 'next/server';
import { authenticateAndRedirect, clearOAuthStateCookie, getOAuthStateCookie, getWorkOSClientId, redirectToCustomAuth } from '../../../../auth/headless-workos';

export const GET = async (request: NextRequest) => {
  const code = request.nextUrl.searchParams.get('code');
  const state = request.nextUrl.searchParams.get('state');
  const stateCookie = getOAuthStateCookie(request);

  if (!code || !state || !stateCookie || stateCookie.state !== state) {
    return redirectToCustomAuth(request, 'signin', 'oauth_state_invalid');
  }

  try {
    const authResponse = await getWorkOS().userManagement.authenticateWithCode({
      clientId: getWorkOSClientId(),
      code,
      ipAddress: request.headers.get('x-forwarded-for')?.split(',')[0]?.trim(),
      userAgent: request.headers.get('user-agent') ?? undefined,
    });

    const response = await authenticateAndRedirect(request, authResponse, stateCookie.returnTo);
    clearOAuthStateCookie(response);

    return response;
  } catch (error) {
    const message = error instanceof Error ? error.message : String(error);
    const status = typeof error === 'object' && error !== null && 'status' in error && typeof error.status === 'number' ? error.status : undefined;
    console.error('Custom WorkOS OAuth callback failed.', { message, status });

    const response = redirectToCustomAuth(request, 'signin', 'oauth_failed', stateCookie.returnTo);
    clearOAuthStateCookie(response);

    return response;
  }
};
