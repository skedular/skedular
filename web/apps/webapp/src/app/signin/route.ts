import { getPublicOrigin } from '@/libs/utils';
import { getSignInUrl } from '@workos-inc/authkit-nextjs';
import { redirect } from 'next/navigation';
import type { NextRequest } from 'next/server';

export const GET = async (request: NextRequest) => {
  const rawReturnTo = request.nextUrl.searchParams.get('returnTo') ?? undefined;
  const returnTo = rawReturnTo?.startsWith('/') ? rawReturnTo : undefined;
  const redirectUri = new URL('/callback', getPublicOrigin(request)).toString();
  const signInUrl = await getSignInUrl({ returnTo, redirectUri });

  return redirect(signInUrl);
};
