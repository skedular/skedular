import { getSignInLink } from '@/components/links';
import { GuestStoreFrontBookings } from '@/components/organizationStoreFrontGuest';
import { OrganizationStoreFrontRootShell, UnauthenticatedOrganizationStoreFrontRootShell } from '@/components/rootShell';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Container from '@mui/material/Container';
import { BodyIconTypography, LeadIconTypography } from '@skedular/ui';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import Link from 'next/link';
import { memo } from 'react';

const RootPage = () => {
  const { user } = useAuth();

  if (user) {
    return (
      <OrganizationStoreFrontRootShell>
        <GuestStoreFrontBookings />
      </OrganizationStoreFrontRootShell>
    );
  }

  return (
    <UnauthenticatedOrganizationStoreFrontRootShell>
      <Container maxWidth="md" sx={{ py: { xs: 6, md: 8 } }}>
        <Card sx={{ borderRadius: 4, border: 1, borderColor: 'divider', boxShadow: 'none' }}>
          <CardContent sx={{ p: { xs: 3, md: 4 } }}>
            <LeadIconTypography label="Sign in to see your bookings" />
            <BodyIconTypography label="Sign in to view your bookings, payment details, and invoices." sx={{ mt: 1, opacity: 0.82 }} />
            <Button component={Link} href={getSignInLink()} variant="contained" sx={{ mt: 2, textTransform: 'none' }}>
              Sign in
            </Button>
          </CardContent>
        </Card>
      </Container>
    </UnauthenticatedOrganizationStoreFrontRootShell>
  );
};

export default memo(RootPage);
