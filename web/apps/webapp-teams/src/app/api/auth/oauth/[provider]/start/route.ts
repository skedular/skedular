import { getWorkOS } from '@workos-inc/authkit-nextjs';
import { NextRequest, NextResponse } from 'next/server';
import { createOAuthStateCookieValue, getCustomAuthRedirectUri, getSafeReturnTo, getWorkOSClientId, setOAuthStateCookie } from '../../../../../auth/headless-workos';

const providers = {
  google: 'GoogleOAuth',
  microsoft: 'MicrosoftOAuth',
} as const;

export const GET = async (request: NextRequest, context: { params: Promise<{ provider: string }> }) => {
  const { provider: providerKey } = await context.params;
  const provider = providers[providerKey as keyof typeof providers];
  if (!provider) {
    return NextResponse.redirect(new URL('/auth/signin?error=unsupported_provider', request.url));
  }

  const returnTo = getSafeReturnTo(request.nextUrl.searchParams.get('returnTo'));
  const stateCookie = createOAuthStateCookieValue(returnTo);
  const authorizationUrl = getWorkOS().userManagement.getAuthorizationUrl({
    clientId: getWorkOSClientId(),
    provider,
    redirectUri: getCustomAuthRedirectUri(request),
    state: stateCookie.state,
  });

  const response = NextResponse.redirect(authorizationUrl);
  setOAuthStateCookie(response, request, stateCookie);

  return response;
};
