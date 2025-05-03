import { MyDetails } from '@/components/myDetails';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const RootPage = () => {
  const handleReloadRequired = () => {};

  return (
    <RootShell hideOrganizationSelector>
      <MyDetails onReloadRequired={handleReloadRequired} />
    </RootShell>
  );
};

export default memo(RootPage);
