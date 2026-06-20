import { SkedularCoreCoreV1Client } from '@/clients/openapi/skedular/v1/core/core/fetch';
import { authkit } from '@workos-inc/authkit-nextjs';
import { Buffer } from 'buffer';
import { NextRequest, NextResponse } from 'next/server';
import { applyAuthkitResponseHeaders, getForwardedAuthorizationHeader } from '../../authkit-response-headers';

const handler = async (request: NextRequest) => {
  const formData = await request.formData();
  const file = formData.get('file');
  if (!(file instanceof Blob)) {
    throw new Error('No file uploaded');
  }

  const { headers: authkitHeaders, session } = await authkit(request);
  const authorization = request.headers.get('Authorization');

  const headers: Record<string, string> = {
    'X-SSO-Cookies': Buffer.from(JSON.stringify(request.cookies.getAll().filter((item) => item.name.startsWith('organization-sso'))), 'binary').toString('base64'),
  };
  const forwardedAuthorization = getForwardedAuthorizationHeader(authorization, session.accessToken);
  if (forwardedAuthorization) {
    headers.Authorization = forwardedAuthorization;
  }

  const client = new SkedularCoreCoreV1Client({ BASE: process.env.GATEWAY_ENDPOINT, HEADERS: headers });

  return applyAuthkitResponseHeaders(
    request,
    NextResponse.json(
      await client.core.uploadPrivateAccessFile({
        file,
      }),
    ),
    authkitHeaders,
  );
};

export { handler as POST };
