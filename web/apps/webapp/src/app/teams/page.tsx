'use client';

import { RootShell } from '@/components/rootShell';
import { OldTeams } from '@/components/team/teams';
import { SwitchToModernUIContext } from '@repo/shared/libs/providers';
import { memo, useContext } from 'react';

const TeamsPage = () => {
  const switchToModernUI = useContext(SwitchToModernUIContext);

  return (
    <RootShell>
      {!switchToModernUI && <OldTeams />}
      {switchToModernUI && <></>}
    </RootShell>
  );
};

export default memo(TeamsPage);
