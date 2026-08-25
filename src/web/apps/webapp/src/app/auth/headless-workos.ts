import { getPublicOrigin } from '@skedular/shared';
import { getWorkOS, saveSession } from '@workos-inc/authkit-nextjs';
import { NextRequest, NextResponse } from 'next/server';

const oauthStateCookieName = 'skedular-workos-oauth-state';
const defaultReturnTo = '/';

type OAuthStateCookie = {
  state: string;
  returnTo: string;
};

export const getSafeReturnTo = (value: FormDataEntryValue | string | null | undefined) => {
  if (typeof value !== 'string' || !value.startsWith('/') || value.startsWith('//')) {
    return defaultReturnTo;
  }

  return value;
};

export const getWorkOSClientId = () => {
  const clientId = process.env.WORKOS_CLIENT_ID;
  if (!clientId) {
    throw new Error('WORKOS_CLIENT_ID must be configured for custom authentication.');
  }

  return clientId;
};

const getRequestPublicUrl = (request: NextRequest, path: string) => new URL(path, getPublicOrigin(request));

export const getCustomAuthRedirectUri = (request: NextRequest) => getRequestPublicUrl(request, '/api/auth/oauth/callback').toString();

export const redirectToCustomAuth = (request: NextRequest, mode: 'signin' | 'signup', error: string, returnTo = defaultReturnTo) => {
  const url = getRequestPublicUrl(request, mode === 'signup' ? '/auth/signup' : '/auth/signin');
  url.searchParams.set('error', error);

  if (returnTo !== defaultReturnTo) {
    url.searchParams.set('returnTo', returnTo);
  }

  return NextResponse.redirect(url);
};

export const authenticateAndRedirect = async (
  request: NextRequest,
  authResponse: Awaited<ReturnType<ReturnType<typeof getWorkOS>['userManagement']['authenticateWithPassword']>>,
  returnTo: string,
) => {
  const { accessToken, refreshToken, user, impersonator, authenticationMethod } = authResponse;
  await saveSession({ accessToken, refreshToken, user, impersonator, authenticationMethod }, request);

  return NextResponse.redirect(getRequestPublicUrl(request, returnTo));
};

export const createOAuthStateCookieValue = (returnTo: string): OAuthStateCookie => ({
  state: crypto.randomUUID(),
  returnTo,
});

export const serialiseOAuthStateCookie = (stateCookie: OAuthStateCookie) => Buffer.from(JSON.stringify(stateCookie), 'utf8').toString('base64url');

export const parseOAuthStateCookie = (value: string | undefined): OAuthStateCookie | null => {
  if (!value) {
    return null;
  }

  try {
    const parsed = JSON.parse(Buffer.from(value, 'base64url').toString('utf8')) as Partial<OAuthStateCookie>;
    if (typeof parsed.state !== 'string') {
      return null;
    }

    return {
      state: parsed.state,
      returnTo: getSafeReturnTo(parsed.returnTo),
    };
  } catch {
    return null;
  }
};

export const setOAuthStateCookie = (response: NextResponse, request: NextRequest, value: OAuthStateCookie) => {
  const publicOrigin = getPublicOrigin(request);

  response.cookies.set(oauthStateCookieName, serialiseOAuthStateCookie(value), {
    httpOnly: true,
    maxAge: 10 * 60,
    path: '/',
    sameSite: 'lax',
    secure: new URL(publicOrigin).protocol === 'https:',
  });
};

export const clearOAuthStateCookie = (response: NextResponse) => {
  response.cookies.set(oauthStateCookieName, '', {
    httpOnly: true,
    maxAge: 0,
    path: '/',
    sameSite: 'lax',
  });
};

export const getOAuthStateCookie = (request: NextRequest) => parseOAuthStateCookie(request.cookies.get(oauthStateCookieName)?.value);
