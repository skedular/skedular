'use client';

import { CustomerSettings } from '@/components/customer/settings';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const Settings = () => (
  <RootShell>
    <CustomerSettings />
  </RootShell>
);

export default memo(Settings);
