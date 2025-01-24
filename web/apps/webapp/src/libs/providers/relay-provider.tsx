'use client';

import { getEnvironment } from '@repo/shared/clients/graphql/skedular';
import { signIn, useSession } from 'next-auth/react';
import { usePathname } from 'next/navigation';
import type { PropsWithChildren } from 'react';
import { useMemo } from 'react';
import { RelayEnvironmentProvider } from 'react-relay/hooks';

interface SessionExtended {
  accessToken?: string;
}

const RelayProvider = ({ children }: PropsWithChildren) => {
  const pathName = usePathname();
  const { data: session } = useSession();

  const environment = useMemo(() => {
    if (!session) {
      return null;
    }

    const sessionExtended = session as any & SessionExtended;

    if (sessionExtended?.error === 'RefreshAccessTokenError') {
      signIn();
    }

    return getEnvironment('/api/graphql', null);
  }, [session]);

  if (environment === null) {
    return pathName === '/signin' ? <>{children}</> : <></>;
  }

  return (
    <RelayEnvironmentProvider environment={environment}>
      <>{children}</>
    </RelayEnvironmentProvider>
  );
};

export default RelayProvider;
