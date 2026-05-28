import { applyResponseHeaders, partitionAuthkitHeaders } from '@workos-inc/authkit-nextjs';
import type { NextRequest, NextResponse } from 'next/server';

export const applyAuthkitResponseHeaders = (request: NextRequest, response: NextResponse, authkitHeaders: Headers) =>
  applyResponseHeaders(response, partitionAuthkitHeaders(request, authkitHeaders).responseHeaders);

export const getForwardedAuthorizationHeader = (requestAuthorization: string | null, sessionAccessToken: string | undefined) =>
  sessionAccessToken ? `Bearer ${sessionAccessToken}` : (requestAuthorization ?? undefined);
