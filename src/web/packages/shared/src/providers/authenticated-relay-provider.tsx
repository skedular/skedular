'use client';

import type { PropsWithChildren } from 'react';
import { useContext } from 'react';
import { InMsTeamsContext } from './in-msteams-provider';
import RelayProvider from './relay-provider';

type Props = PropsWithChildren<{
  accessToken?: string;
  accessTokenLoading?: boolean;
  authLoading?: boolean;
  teamsToken?: string | null;
  userSignedIn?: boolean;
}>;

const AuthenticatedRelayProvider = ({ accessToken, accessTokenLoading = false, authLoading = false, children, teamsToken, userSignedIn = false }: Props) => {
  const inMsTeams = useContext(InMsTeamsContext);

  if (!inMsTeams && (authLoading || (userSignedIn && accessTokenLoading))) {
    return null;
  }

  return <RelayProvider token={inMsTeams ? teamsToken : accessToken}>{children}</RelayProvider>;
};

export default AuthenticatedRelayProvider;
