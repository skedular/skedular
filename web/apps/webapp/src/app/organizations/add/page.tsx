'use client';

import { AddOrganization } from '@/components/organization/addOrganization';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const AddOrganizationPage = () => (
  <RootShell>
    <AddOrganization />
  </RootShell>
);

export default memo(AddOrganizationPage);
