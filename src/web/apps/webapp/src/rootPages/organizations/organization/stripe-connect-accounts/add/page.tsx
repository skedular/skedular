import { RootShell } from '@/components/rootShell';
import { AddStripeConnectAccount } from '@/components/stripeConnectAccount/addStripeConnectAccount';
import { useRouter } from 'next/navigation';
import { memo } from 'react';
import useKnownParams from '@/hooks/use-known-params';

const RootPage = () => {
  const router = useRouter();
  const { organizationCustomDomain } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
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
      <AddStripeConnectAccount onReloadRequired={handleReloadRequired} onAdded={handleAdded} onCancel={handleCancelled} organizationCustomDomain={organizationCustomDomain} />
    </RootShell>
  );
};

export default memo(RootPage);
