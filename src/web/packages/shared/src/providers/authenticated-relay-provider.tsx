'use client';

import type { PropsWithChildren } from 'react';
import { useContext } from 'react';
import { InMsTeamsContext } from './in-msteams-provider';
import RelayProvider from './relay-provider';

type Props = PropsWithChildren<{
  authLoading?: boolean;
  teamsToken?: string | null;
}>;

const AuthenticatedRelayProvider = ({ authLoading = false, children, teamsToken }: Props) => {
  const inMsTeams = useContext(InMsTeamsContext);

  if (!inMsTeams && authLoading) {
    return null;
  }

  return <RelayProvider token={inMsTeams ? teamsToken : undefined}>{children}</RelayProvider>;
};

export default AuthenticatedRelayProvider;
