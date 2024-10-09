'use client';

import { Locations } from '@/components/location/locations';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const LocationsPage = () => (
  <RootShell>
    <Locations />
  </RootShell>
);

export default memo(LocationsPage);
