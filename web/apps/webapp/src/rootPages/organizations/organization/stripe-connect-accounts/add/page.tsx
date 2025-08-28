import { RootShell } from '@/components/rootShell';
import { AddStripeConnectAccount } from '@/components/stripeConnectAccount/addStripeConnectAccount';
import { useParams, useRouter } from 'next/navigation';
import { memo } from 'react';

const RootPage = () => {
  const router = useRouter();
  const { organizationUniqueAlphanumericName } = useParams();
  let finalOrganizationUniqueAlphanumericName = '';

  if (typeof organizationUniqueAlphanumericName === 'string') {
    finalOrganizationUniqueAlphanumericName = organizationUniqueAlphanumericName;
  } else if (Array.isArray(organizationUniqueAlphanumericName)) {
    if (typeof organizationUniqueAlphanumericName[0] === 'undefined') {
      throw new Error('organizationUniqueAlphanumericName is required');
    }

    finalOrganizationUniqueAlphanumericName = organizationUniqueAlphanumericName[0];
  } else {
    throw new Error('organizationUniqueAlphanumericName is required');
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
      <AddStripeConnectAccount
        onReloadRequired={handleReloadRequired}
        onAdded={handleAdded}
        onCancel={handleCancelled}
        organizationUniqueAlphanumericName={finalOrganizationUniqueAlphanumericName}
      />
    </RootShell>
  );
};

export default memo(RootPage);
