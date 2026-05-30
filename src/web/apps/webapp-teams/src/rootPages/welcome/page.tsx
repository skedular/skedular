import { getOrganizationAddPrivateLink } from '@/components/links';
import { NoOrganizationRootShell } from '@/components/rootShell';
import { SetupFlow } from '@/components/setupFlow';
import { useIntegratedPlatform } from '@skedular/shared';
import { useRouter } from 'next/navigation';
import { memo } from 'react';

const RootPage = () => {
  const { integratedPlatform } = useIntegratedPlatform();
  const router = useRouter();

  return (
    <NoOrganizationRootShell>
      <SetupFlow onUserTypeClick={() => router.push(getOrganizationAddPrivateLink(integratedPlatform))} />
    </NoOrganizationRootShell>
  );
};

export default memo(RootPage);
