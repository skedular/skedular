import { Locations, OldLocations } from 'components/location/locations';
import { RootShell } from 'components/rootShell';
import { memo, useContext } from 'react';
import { useParams } from 'react-router-dom';
import { SwitchToModernUIContext } from '@repo/shared/libs/providers';

const LocationsPage = () => {
  const switchToModernUI = useContext(SwitchToModernUIContext);
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

  return (
    <RootShell>
      {!switchToModernUI && <OldLocations organizationId={finalOrganizationId} />}
      {switchToModernUI && <Locations organizationId={finalOrganizationId} />}
    </RootShell>
  );
};

export default memo(LocationsPage);
