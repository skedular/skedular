import { getOrganizationBaseLink } from '@/components/links';
import { AddOrganization } from '@/components/organization/addOrganization';
import { RootShell } from '@/components/rootShell';
import { useIntegratedPlatrform } from '@/libs/providers';
import { useRouter } from 'next/navigation';
import { memo } from 'react';

const RootPage = () => {
  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();

  const handleAdded = (id: string) => {
    router.push(getOrganizationBaseLink(integratedPlatrform, id));
  };

  const handleCancelled = () => {
    router.back();
  };

  const handleReloadRequired = () => {};

  return (
    <RootShell hideOrganizationSelector>
      <AddOrganization showCancel={true} onAdded={handleAdded} onCancel={handleCancelled} onReloadRequired={handleReloadRequired} />
    </RootShell>
  );
};

export default memo(RootPage);
