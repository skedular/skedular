import { AddBankAccount } from '@/components/bankAccount/addBankAccount';
import { RootShell } from '@/components/rootShell';
import { useParams, useRouter } from 'next/navigation';
import { memo } from 'react';

const RootPage = () => {
  const router = useRouter();
  const { organizationId } = useParams();
  let finalOrganizationId = '';

  if (typeof organizationId === 'string') {
    finalOrganizationId = organizationId;
  } else if (Array.isArray(organizationId)) {
    if (typeof organizationId[0] === 'undefined') {
      throw new Error('organizationId is required');
    }

    finalOrganizationId = organizationId[0];
  } else {
    throw new Error('organizationId is required');
  }

  const handleAdded = () => {
    router.back();
  };

  const handleCancelled = () => {
    router.back();
  };

  const handleReloadRequired = () => {};

  return (
    <RootShell>
      <AddBankAccount onReloadRequired={handleReloadRequired} onAdded={handleAdded} onCancel={handleCancelled} organizationId={finalOrganizationId} />
    </RootShell>
  );
};

export default memo(RootPage);
