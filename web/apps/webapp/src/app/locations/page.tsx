'use client';

import { OldLocations } from '@/components/location/locations';
import { RootShell } from '@/components/rootShell';
import { SwitchToModernUIContext } from '@repo/shared/libs/providers';
import { memo, useContext } from 'react';

const LocationsPage = () => {
  const switchToModernUI = useContext(SwitchToModernUIContext);

  return (
    <RootShell>
      {!switchToModernUI && <OldLocations />}
      {switchToModernUI && <></>}
    </RootShell>
  );
};

export default memo(LocationsPage);
