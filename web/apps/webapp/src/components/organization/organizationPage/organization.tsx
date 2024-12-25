import { SwitchToModernUIContext } from '@repo/shared/libs/providers';
import { memo, useContext } from 'react';
import ModernOrganization from './modern-organization';
import OldOrganization from './old-organization';

type RelayProps = {
  organizationId: string;
};

const Organization = ({ organizationId }: RelayProps) => {
  const switchToModernUI = useContext(SwitchToModernUIContext);

  return switchToModernUI ? <ModernOrganization organizationId={organizationId} /> : <OldOrganization organizationId={organizationId} />;
};

export default memo(Organization);
