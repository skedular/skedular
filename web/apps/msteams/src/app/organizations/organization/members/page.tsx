import { Breadcrumbs } from '@mui/material';
import { BodyIconTypography } from '@repo/shared/components/commons';
import { OrganizationMembers } from 'components/organization/organizationMembers';
import { RootShell } from 'components/rootShell';
import { memo } from 'react';
import { useParams } from 'react-router-dom';

const OrganizationsPage = () => {
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

  const breadcrumbs = (
    <Breadcrumbs>
      <BodyIconTypography label="View Members" />
      <BodyIconTypography label="Organization" />
    </Breadcrumbs>
  );

  return (
    <RootShell collapsed hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <OrganizationMembers organizationId={finalOrganizationId} />
    </RootShell>
  );
};

export default memo(OrganizationsPage);
