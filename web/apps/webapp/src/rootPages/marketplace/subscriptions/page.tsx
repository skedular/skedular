import { BodyIconTypography, LeadIconTypography } from '@/components/commons';
import { getSignInLink } from '@/components/links';
import { GuestStoreFrontSubscriptions } from '@/components/organizationStoreFrontGuest';
import { OrganizationStoreFrontRootShell, UnauthenticatedOrganizationStoreFrontRootShell } from '@/components/rootShell';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Container from '@mui/material/Container';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import Link from 'next/link';
import { memo } from 'react';

const RootPage = () => {
  const { user, loading } = useAuth();

  if (loading) {
    return null;
  }

  if (user) {
    return (
      <OrganizationStoreFrontRootShell>
        <GuestStoreFrontSubscriptions />
      </OrganizationStoreFrontRootShell>
    );
  }

  return (
    <UnauthenticatedOrganizationStoreFrontRootShell>
      <Container maxWidth="md" sx={{ py: { xs: 6, md: 8 } }}>
        <Card sx={{ borderRadius: 4, border: 1, borderColor: 'divider', boxShadow: 'none' }}>
          <CardContent sx={{ p: { xs: 3, md: 4 } }}>
            <LeadIconTypography label="Sign in to see your subscriptions" />
            <BodyIconTypography
              label="You’ll need an account to review active plans, current subscription cycles, and recurring periods from this storefront."
              sx={{ mt: 1, opacity: 0.82 }}
            />
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
