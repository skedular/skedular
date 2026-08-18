'use client';

import { CustomerBookingsHub } from '@/components/booking/myBookings';
import { getSignInLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { NoOrganizationRootShell, OrganizationStoreFrontRootShell, UnauthenticatedOrganizationStoreFrontRootShell, UnauthenticatedRootShell } from '@/components/rootShell';
import useKnownParams from '@/hooks/use-known-params';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Container from '@mui/material/Container';
import { BodyIconTypography, LeadIconTypography } from '@skedular/ui';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import Link from 'next/link';
import { memo } from 'react';

const RootPage = () => {
  const { user, loading } = useAuth();
  const { isCustomDomain } = useKnownParams();
  if (loading) {
    return <Loading />;
  }

  if (user) {
    return isCustomDomain ? (
      <OrganizationStoreFrontRootShell>
        <CustomerBookingsHub />
      </OrganizationStoreFrontRootShell>
    ) : (
      <NoOrganizationRootShell>
        <CustomerBookingsHub />
      </NoOrganizationRootShell>
    );
  }

  const content = (
    <Container maxWidth="md" sx={{ py: { xs: 6, md: 8 } }}>
      <Card
        sx={{
          borderRadius: 4,
          border: 1,
          borderColor: 'divider',
          boxShadow: 'none',
        }}
      >
        <CardContent sx={{ p: { xs: 3, md: 4 } }}>
          <LeadIconTypography label="Sign in to see your bookings" />
          <BodyIconTypography label="Sign in to view your bookings, payment details, and invoices." sx={{ mt: 1, opacity: 0.82 }} />
          <Button component={Link} href={getSignInLink()} variant="contained" sx={{ mt: 2, textTransform: 'none' }}>
            Sign in
          </Button>
        </CardContent>
      </Card>
    </Container>
  );

  return isCustomDomain ? (
    <UnauthenticatedOrganizationStoreFrontRootShell>{content}</UnauthenticatedOrganizationStoreFrontRootShell>
  ) : (
    <UnauthenticatedRootShell>{content}</UnauthenticatedRootShell>
  );
};

export default memo(RootPage);
