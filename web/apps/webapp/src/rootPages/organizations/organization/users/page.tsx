import { BodyIconTypography, StackColumn } from '@/components/commons';
import { OrganizationUsers } from '@/components/organization/organizationUsers';
import { RootShell } from '@/components/rootShell';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { useParams, useRouter } from 'next/navigation';
import { memo } from 'react';

const OrganizationsPage = () => {
  const router = useRouter();
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

  const handleBackClick = () => {
    router.back();
  };

  const breadcrumbs = (
    <StackColumn sx={{ alignItems: 'flex-start' }} spacing={0}>
      <Button variant="text" onClick={handleBackClick} sx={{ whiteSpace: 'nowrap', textTransform: 'none' }}>
        {'< back'}
      </Button>
      <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
        <Breadcrumbs>
          <BodyIconTypography label="View Users" />
        </Breadcrumbs>
      </Box>
    </StackColumn>
  );

  return (
    <RootShell collapsed hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <OrganizationUsers organizationUniqueAlphanumericName={finalOrganizationUniqueAlphanumericName} />
    </RootShell>
  );
};

export default memo(OrganizationsPage);
