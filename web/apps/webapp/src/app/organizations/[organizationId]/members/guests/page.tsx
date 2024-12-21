'use client';

import { OrganizationMembers } from '@/components/organization/organizationMembers';
import { RootShell } from '@/components/rootShell';
import { Breadcrumbs } from '@mui/material';
import { BodyIconTypography } from '@repo/shared/components/commons';
import { useParams } from 'next/navigation';
import { memo } from 'react';

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
      <BodyIconTypography label="View Guests" />
      <BodyIconTypography label="Organization" />
    </Breadcrumbs>
  );

  return (
    <RootShell collapsed hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <OrganizationMembers organizationId={finalOrganizationId} />
    </RootShell>
  );
};

export default memo(OrganizationsPage);
