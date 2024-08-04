'use client';

import { signIn, useSession } from 'next-auth/react';
import { useMemo } from 'react';
import { RelayEnvironmentProvider } from 'react-relay/hooks';
import { getEnvironment } from '../../clients/graphql/unityhub';

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

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const sessionExtended = session as any & SessionExtended;

    if (sessionExtended?.error === 'RefreshAccessTokenError') {
      signIn();
    }

    return getEnvironment(null);
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
