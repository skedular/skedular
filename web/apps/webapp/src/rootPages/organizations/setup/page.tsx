import { getOrganizationAddMarketplaceLink, getOrganizationAddPrivateLink } from '@/components/links';
import { NoOrganizationRootShell } from '@/components/rootShell';
import type { UserType } from '@/components/setupFlow';
import { SetupFlow } from '@/components/setupFlow';
import { useIntegratedPlatrform } from '@skedular/shared';
import { useRouter } from 'next/navigation';
import { memo } from 'react';

const RootPage = () => {
  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();

  const handleUserTypeClick = (userType: UserType) => {
    switch (userType) {
      case 'private':
        router.push(getOrganizationAddPrivateLink(integratedPlatrform));
        break;

      case 'marketplace':
        router.push(getOrganizationAddMarketplaceLink(integratedPlatrform));
        break;
    }
  };

  return (
    <NoOrganizationRootShell hideOrganizationSelector>
      <SetupFlow onUserTypeClick={handleUserTypeClick} userTypesToShow={['private', 'marketplace']} showBackButton />
    </NoOrganizationRootShell>
  );
};

export default memo(RootPage);
