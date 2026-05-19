import { MySettings } from '@/components/mySettings';
import { NoOrganizationRootShell } from '@/components/rootShell';
import { memo } from 'react';

const RootPage = () => {
  const handleReloadRequired = () => {};

  return (
    <NoOrganizationRootShell hideOrganizationSelector>
      <MySettings onReloadRequired={handleReloadRequired} />
    </NoOrganizationRootShell>
  );
};

export default memo(RootPage);
