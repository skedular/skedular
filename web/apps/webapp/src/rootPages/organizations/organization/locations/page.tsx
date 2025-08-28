import { OrganizationLocations } from '@/components/organization/organizationLocations';
import { RootShell } from '@/components/rootShell';
import { useParams } from 'next/navigation';
import { memo } from 'react';

const RootPage = () => {
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

  return (
    <RootShell>
      <OrganizationLocations organizationUniqueAlphanumericName={finalOrganizationUniqueAlphanumericName} />
    </RootShell>
  );
};

export default memo(RootPage);
