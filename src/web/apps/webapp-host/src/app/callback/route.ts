import { getPublicOrigin } from '@skedular/shared';
import { handleAuth } from '@workos-inc/authkit-nextjs';
import type { NextRequest } from 'next/server';

export const GET = async (request: NextRequest) => handleAuth({ baseURL: getPublicOrigin(request) })(request);
