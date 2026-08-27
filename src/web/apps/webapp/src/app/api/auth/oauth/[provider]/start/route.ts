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
  const pkce = await getWorkOS().pkce.generate();
  const stateCookie = createOAuthStateCookieValue(returnTo, pkce.codeVerifier);
  console.info('Starting custom WorkOS OAuth flow.', {
    provider: providerKey,
    hasClientId: Boolean(process.env.WORKOS_CLIENT_ID),
    hasApiKey: Boolean(process.env.WORKOS_API_KEY),
    hasCodeVerifier: Boolean(pkce.codeVerifier),
    redirectHost: new URL(getCustomAuthRedirectUri(request)).host,
  });
  const authorizationUrl = getWorkOS().userManagement.getAuthorizationUrl({
    clientId: getWorkOSClientId(),
    provider,
    redirectUri: getCustomAuthRedirectUri(request),
    state: stateCookie.state,
    codeChallenge: pkce.codeChallenge,
    codeChallengeMethod: pkce.codeChallengeMethod,
  });

  const response = NextResponse.redirect(authorizationUrl);
  setOAuthStateCookie(response, request, stateCookie);

  return response;
};
