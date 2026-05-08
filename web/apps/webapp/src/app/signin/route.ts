import { getPublicOrigin } from '@skedular/shared';
import { getSignInUrl } from '@workos-inc/authkit-nextjs';
import { redirect } from 'next/navigation';
import type { NextRequest } from 'next/server';
import { getCustomDomainOrganizationName } from '../auth/custom-domain';

export const GET = async (request: NextRequest) => {
  const rawReturnTo = request.nextUrl.searchParams.get('returnTo') ?? undefined;
  const returnTo = rawReturnTo?.startsWith('/') ? rawReturnTo : undefined;
  const loginHint = request.nextUrl.searchParams.get('loginHint') ?? undefined;
  const organizationCustomDomain = getCustomDomainOrganizationName(request);

  if (organizationCustomDomain) {
    const searchParams = new URLSearchParams();
    if (returnTo) {
      searchParams.set('returnTo', returnTo);
    }

    const query = searchParams.toString();
    return redirect(`/auth/signin${query ? `?${query}` : ''}`);
  }

  const redirectUri = new URL('/callback', getPublicOrigin(request)).toString();
  const signInUrl = await getSignInUrl({ returnTo, redirectUri, loginHint });

  return redirect(signInUrl);
};
