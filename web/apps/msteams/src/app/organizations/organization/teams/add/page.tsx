import { RootShell } from 'components/rootShell';
import { AddTeam } from 'components/team/addTeam';
import { memo } from 'react';
import { useNavigate, useParams } from 'react-router-dom';

const AddTeamPage = () => {
  const navigate = useNavigate();
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
    navigate(-1);
  };

  const handleCancelled = () => {
    navigate(-1);
  };

  const handleReloadRequired = () => {};

  return (
    <RootShell>
      <AddTeam organizationId={finalOrganizationId} onAdded={handleAdded} onCancelled={handleCancelled} onReloadRequired={handleReloadRequired} />
    </RootShell>
  );
};

export default memo(AddTeamPage);
