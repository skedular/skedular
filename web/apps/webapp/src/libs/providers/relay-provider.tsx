import { getEnvironment } from '@/clients/graphql/skedular';
import { InMsTeamsContext } from '@/libs/providers';
import type { PropsWithChildren } from 'react';
import { useContext, useMemo } from 'react';
import { RelayEnvironmentProvider } from 'react-relay/hooks';

type Props = {
  token?: string | null | undefined;
};

const RelayProvider = ({ children, token }: PropsWithChildren<Props>) => {
  const inMsTeams = useContext(InMsTeamsContext);
  const environment = useMemo(() => (inMsTeams && !token ? null : getEnvironment('/api/v1/graphql', token)), [token, inMsTeams]);

  if (!environment) {
    return null;
  }

  return (
    <RelayEnvironmentProvider environment={environment}>
      <>{children}</>
    </RelayEnvironmentProvider>
  );
};

export default RelayProvider;
