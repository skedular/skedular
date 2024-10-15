'use client';

import { getEnvironment } from '@repo/shared/clients/graphql/unityhub';
import { signIn, useSession } from 'next-auth/react';
import { useMemo } from 'react';
import { RelayEnvironmentProvider } from 'react-relay/hooks';

type Props = {
  children?: React.ReactNode;
};

interface SessionExtended {
  accessToken?: string;
}

const RelayProvider = ({ children }: Props) => {
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
    return <></>;
  }

  return (
    <RelayEnvironmentProvider environment={environment}>
      <>{children}</>
    </RelayEnvironmentProvider>
  );
};

export default RelayProvider;
