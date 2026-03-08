import { AddMarketplaceLocation } from '@/components/location/addLocation';
import { RootShell } from '@/components/rootShell';
import { useKnownParams } from '@/libs/providers';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo } from 'react';

const RootPage = () => {
  const searchParams = useSearchParams();
  const redirectUrl = searchParams.get('redirectUrl');
  const router = useRouter();
  const { organizationUniqueAlphanumericName } = useKnownParams();

  if (!organizationUniqueAlphanumericName) {
    throw new Error('organizationUniqueAlphanumericName is required');
  }

  const handleAdded = () => {
    if (redirectUrl) {
      router.push(redirectUrl);
    } else {
      router.back();
    }
  };

  const handleCancelled = () => {
    if (redirectUrl) {
      router.push(redirectUrl);
    } else {
      router.back();
    }
  };

  const handleReloadRequired = () => {};

  return (
    <RootShell collapsed>
      <AddMarketplaceLocation
        organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
        onAdded={handleAdded}
        onCancel={handleCancelled}
        onReloadRequired={handleReloadRequired}
      />
    </RootShell>
  );
};

export default memo(RootPage);
