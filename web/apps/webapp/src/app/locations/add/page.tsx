'use client';

import { AddLocation } from '@/components/location/addLocation';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const AddLocationPage = () => (
  <RootShell>
    <AddLocation />
  </RootShell>
);

export default memo(AddLocationPage);
