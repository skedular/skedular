import { getEnvironment } from '@repo/shared/clients/graphql/unityhub';
import { useMemo } from 'react';
import { RelayEnvironmentProvider } from 'react-relay/hooks';

type Props = {
  children?: React.ReactNode;
  token: string | null;
};

const RelayProvider = ({ children, token }: Props) => {
  const environment = useMemo(() => {
    if (!token) {
      return null;
    }

    return getEnvironment(process.env.REACT_APP_GRAPHQL_ENDPOINT!, `Bearer ${token}`);
  }, [token]);

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
