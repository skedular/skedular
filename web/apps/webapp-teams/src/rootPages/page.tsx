import { getSignInLink } from '@/components/links';
import { NoOrganizationRootShell } from '@/components/rootShell';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import { BodyIconTypography, LeadIconTypography, StackColumn } from '@skedular/ui';
import { memo } from 'react';

const RootPage = () => {
  const { user } = useAuth();

  return (
    <NoOrganizationRootShell collapsed hideWelcomeMessage={!user}>
      <StackColumn sx={{ p: { xs: 2, md: 4 }, maxWidth: 760 }}>
        <Card variant="outlined">
          <CardContent>
            <StackColumn>
              <LeadIconTypography label="Select a private organisation" />
              <BodyIconTypography label="Teams is for private organisations, team membership, users, bookings, locations, and internal availability workflows." />
              {!user && (
                <Button href={getSignInLink()} variant="contained" sx={{ alignSelf: 'flex-start', textTransform: 'none' }}>
                  Sign in
                </Button>
              )}
            </StackColumn>
          </CardContent>
        </Card>
      </StackColumn>
    </NoOrganizationRootShell>
  );
};

export default memo(RootPage);
