import { BodyIconTypography, StackColumn } from '@skedular/ui';
import { OrganizationUsers } from '@/components/organization/organizationUsers';
import { RootShell } from '@/components/rootShell';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { useRouter } from 'next/navigation';
import { memo } from 'react';
import useKnownParams from '@/hooks/use-known-params';

const OrganizationsPage = () => {
  const router = useRouter();
  const { organizationCustomDomain } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
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
    <RootShell hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <OrganizationUsers organizationCustomDomain={organizationCustomDomain} />
    </RootShell>
  );
};

export default memo(OrganizationsPage);
