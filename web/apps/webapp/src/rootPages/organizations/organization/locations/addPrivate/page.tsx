import { AddPrivateLocation } from '@/components/location/addLocation';
import { RootShell } from '@/components/rootShell';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo } from 'react';
import useKnownParams from '@/hooks/use-known-params';

const RootPage = () => {
  const searchParams = useSearchParams();
  const redirectUrl = searchParams.get('redirectUrl');
  const router = useRouter();
  const { organizationCustomDomain } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
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
      <AddPrivateLocation organizationCustomDomain={organizationCustomDomain} onAdded={handleAdded} onCancel={handleCancelled} onReloadRequired={handleReloadRequired} />
    </RootShell>
  );
};

export default memo(RootPage);
