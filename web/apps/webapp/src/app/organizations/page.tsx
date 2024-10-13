'use client';

import { Organizations } from '@/components/organization/organizations';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const OrganizationsPage = () => (
  <RootShell>
    <Organizations />
  </RootShell>
);

export default memo(OrganizationsPage);
