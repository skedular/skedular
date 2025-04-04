import { getEnvironment } from '@/clients/graphql/skedular';
import type { PropsWithChildren } from 'react';
import { useMemo } from 'react';
import { RelayEnvironmentProvider } from 'react-relay/hooks';

type Props = {
  token?: string | null | undefined;
};

const RelayProvider = ({ children, token }: PropsWithChildren<Props>) => {
  //  const isRunningInTeams = () => typeof window !== 'undefined' && window.name === 'embedded-page-container';
  const isRunningInTeams = () => false;
  const environment = useMemo(() => (isRunningInTeams() && !token ? null : getEnvironment('/api/graphql', token)), [token]);

  if (!environment) {
    return <></>;
  }

  return (
    <RelayEnvironmentProvider environment={environment}>
      <>{children}</>
    </RelayEnvironmentProvider>
  );
};

export default RelayProvider;
