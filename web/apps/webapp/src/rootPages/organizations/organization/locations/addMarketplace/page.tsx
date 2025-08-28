import { AddMarketplaceLocation } from '@/components/location/addLocation';
import { RootShell } from '@/components/rootShell';
import { useParams, useRouter, useSearchParams } from 'next/navigation';
import { memo } from 'react';

const RootPage = () => {
  const searchParams = useSearchParams();
  const redirectUrl = searchParams.get('redirectUrl');
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
        organizationUniqueAlphanumericName={finalOrganizationUniqueAlphanumericName}
        onAdded={handleAdded}
        onCancel={handleCancelled}
        onReloadRequired={handleReloadRequired}
      />
    </RootShell>
  );
};

export default memo(RootPage);
