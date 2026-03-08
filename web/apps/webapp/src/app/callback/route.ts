import { getPublicOrigin } from '@/libs/utils';
import { handleAuth } from '@workos-inc/authkit-nextjs';
import type { NextRequest } from 'next/server';

export const GET = async (request: NextRequest) => {
  return handleAuth({ baseURL: getPublicOrigin(request) })(request);
};
