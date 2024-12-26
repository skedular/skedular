import { getEnvironment } from '@repo/shared/clients/graphql/skedular';
import type { PropsWithChildren } from 'react';
import { useMemo } from 'react';
import { RelayEnvironmentProvider } from 'react-relay/hooks';

type Props = {
  token: string | null;
};

const RelayProvider = ({ children, token }: PropsWithChildren<Props>) => {
  const environment = useMemo(() => {
    if (!token) {
      return null;
    }

    return getEnvironment(new URL('v1/graphql', process.env.REACT_APP_GATEWAY_ENDPOINT!).href, `Bearer ${token}`);
  }, [token]);

  if (environment === null) {
    return <></>;
  }

  return <RelayEnvironmentProvider environment={environment}>{children}</RelayEnvironmentProvider>;
};

export default RelayProvider;
