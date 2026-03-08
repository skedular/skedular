import { RootShell } from '@/components/rootShell';
import { AddStripeConnectAccount } from '@/components/stripeConnectAccount/addStripeConnectAccount';
import { useKnownParams } from '@/libs/providers';
import { useRouter } from 'next/navigation';
import { memo } from 'react';

const RootPage = () => {
  const router = useRouter();
  const { organizationUniqueAlphanumericName } = useKnownParams();

  if (!organizationUniqueAlphanumericName) {
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
        organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
      />
    </RootShell>
  );
};

export default memo(RootPage);
