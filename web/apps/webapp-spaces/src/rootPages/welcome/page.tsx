import { getOrganizationAddMarketplaceLink } from '@/components/links';
import { NoOrganizationRootShell } from '@/components/rootShell';
import { SetupFlow } from '@/components/setupFlow';
import { useIntegratedPlatrform } from '@skedular/shared';
import { useRouter } from 'next/navigation';
import { memo } from 'react';

const RootPage = () => {
  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();

  return (
    <NoOrganizationRootShell hideOrganizationSelector collapsed>
      <SetupFlow onUserTypeClick={() => router.push(getOrganizationAddMarketplaceLink(integratedPlatrform))} />
    </NoOrganizationRootShell>
  );
};

export default memo(RootPage);
