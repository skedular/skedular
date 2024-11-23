'use client';

import { Dashboard } from '@/components/customer/dashboard';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const Home = () => (
  <RootShell>
    <Dashboard />
  </RootShell>
);

export default memo(Home);
