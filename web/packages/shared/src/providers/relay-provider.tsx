'use client';

import type { PropsWithChildren } from 'react';
import { useContext, useMemo } from 'react';
// eslint-disable-next-line @typescript-eslint/ban-ts-comment
// @ts-ignore react-relay v20 ships without bundled .d.ts
import { RelayEnvironmentProvider } from 'react-relay';
import { getEnvironment } from '../utils/relay-environment';
import { InMsTeamsContext } from './in-msteams-provider';

type Props = {
  token?: string | null | undefined;
  graphqlEndpoint?: string;
};

const RelayProvider = ({ children, token, graphqlEndpoint = '/api/v1/graphql' }: PropsWithChildren<Props>) => {
  const inMsTeams = useContext(InMsTeamsContext);
  const environment = useMemo(() => (inMsTeams && !token ? null : getEnvironment(graphqlEndpoint, token)), [token, inMsTeams, graphqlEndpoint]);

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
