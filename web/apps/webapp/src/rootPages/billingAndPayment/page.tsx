import { MyBillingAndPayment } from '@/components/myBillingAndPayment';
import { NoOrganizationRootShell } from '@/components/rootShell';
import { memo } from 'react';

const RootPage = () => {
  const handleReloadRequired = () => {};

  return (
    <NoOrganizationRootShell hideOrganizationSelector>
      <MyBillingAndPayment onReloadRequired={handleReloadRequired} />
    </NoOrganizationRootShell>
  );
};

export default memo(RootPage);
