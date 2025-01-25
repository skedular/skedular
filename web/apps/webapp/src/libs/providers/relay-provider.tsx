import { getEnvironment } from '@repo/shared/clients/graphql/skedular';
import type { PropsWithChildren } from 'react';
import { useMemo } from 'react';
import { RelayEnvironmentProvider } from 'react-relay/hooks';

interface SessionExtended {
  accessToken?: string;
}

const RelayProvider = ({ children }: PropsWithChildren) => {
  const environment = useMemo(() => getEnvironment('/api/graphql', null), []);

  return (
    <RelayEnvironmentProvider environment={environment}>
      <>{children}</>
    </RelayEnvironmentProvider>
  );
};

export default RelayProvider;
