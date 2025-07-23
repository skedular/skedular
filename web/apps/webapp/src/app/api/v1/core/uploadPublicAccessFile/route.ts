import { SkedularCoreV1Client } from '@/clients/openapi/skedular/v1/core/fetch';
import { authkit } from '@workos-inc/authkit-nextjs';
import { Buffer } from 'buffer';
import { NextRequest } from 'next/server';

const handler = async (request: NextRequest) => {
  const formData = await request.formData();
  const file = formData.get('file');
  if (!(file instanceof Blob)) {
    throw new Error('No file uploaded');
  }

  const { session } = await authkit(request);
  const authorization = request.headers.get('Authorization');

  const headers = {
    Authorization: authorization ? authorization : `Bearer ${session.accessToken}`,
    'X-SSO-Cookies': Buffer.from(JSON.stringify(request.cookies.getAll().filter((item) => item.name.startsWith('organization-sso'))), 'binary').toString('base64'),
  };

  const client = new SkedularCoreV1Client({ BASE: process.env.GATEWAY_ENDPOINT, HEADERS: headers });

  return Response.json(
    await client.core.uploadPublicAccessFile({
      file,
    }),
  );
};

export { handler as POST };
