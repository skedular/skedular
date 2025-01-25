'use server';

import { getSignInUrl, getSignUpUrl } from '@workos-inc/authkit-nextjs';

export const getSignInUrlAction = async () => {
  return await getSignInUrl();
};

export const getSignUpUrlAction = async () => {
  return await getSignUpUrl();
};
