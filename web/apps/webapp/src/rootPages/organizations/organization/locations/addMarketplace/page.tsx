import { AddMarketplaceLocation } from '@/components/location/addLocation';
import { RootShell } from '@/components/rootShell';
import { useParams, useRouter, useSearchParams } from 'next/navigation';
import { memo } from 'react';

const RootPage = () => {
  const searchParams = useSearchParams();
  const redirectUrl = searchParams.get('redirectUrl');
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
      <AddMarketplaceLocation organizationId={finalOrganizationId} onAdded={handleAdded} onCancel={handleCancelled} onReloadRequired={handleReloadRequired} />
    </RootShell>
  );
};

export default memo(RootPage);
