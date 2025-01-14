import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { BodyIconTypography, StackColumn } from '@repo/shared/components/commons';
import { OrganizationUsers } from 'components/organization/organizationUsers';
import { RootShell } from 'components/rootShell';
import { memo } from 'react';
import { useNavigate, useParams } from 'react-router-dom';

const OrganizationsPage = () => {
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

  const handleBackClick = () => {
    navigate(-1);
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
    <RootShell collapsed hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <OrganizationUsers organizationId={finalOrganizationId} />
    </RootShell>
  );
};

export default memo(OrganizationsPage);
