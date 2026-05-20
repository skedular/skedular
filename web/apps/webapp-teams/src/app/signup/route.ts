import { getPublicOrigin } from '@skedular/shared';
import { getSignUpUrl } from '@workos-inc/authkit-nextjs';
import { redirect } from 'next/navigation';
import type { NextRequest } from 'next/server';

export const GET = async (request: NextRequest) => {
  const rawReturnTo = request.nextUrl.searchParams.get('returnTo') ?? undefined;
  const returnTo = rawReturnTo?.startsWith('/') ? rawReturnTo : undefined;
  const loginHint = request.nextUrl.searchParams.get('loginHint') ?? undefined;
  const redirectUri = new URL('/callback', getPublicOrigin(request)).toString();
  const signUpUrl = await getSignUpUrl({ returnTo, redirectUri, loginHint });

  return redirect(signUpUrl);
};
