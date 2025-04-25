'use client';

import { MyDetails } from '@/components/myDetails';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const MePage = () => {
  const handleReloadRequired = () => {};

  return (
    <RootShell hideOrganizationSelector>
      <MyDetails onReloadRequired={handleReloadRequired} />
    </RootShell>
  );
};

export default memo(MePage);
