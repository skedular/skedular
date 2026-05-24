import { NoOrganizationLandingContent, NoOrganizationLandingPageRootQuery } from '@/components/noOrganizationLanding';
import type { noOrganizationLandingPage_rootQuery } from '@/queries/__generated__/noOrganizationLandingPage_rootQuery.graphql';
import { NoOrganizationRootShell } from '@/components/rootShell';
import { RelayError, toRootError } from '@/components/relayError';
import { getSignInLink } from '@/components/links';
import CircularProgress from '@mui/material/CircularProgress';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import { BodyIconTypography, LeadIconTypography, StackColumn } from '@skedular/ui';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import { memo, Suspense, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { useQueryLoader } from 'react-relay';

const RootPage = () => {
  const [queryRef, loadQuery] = useQueryLoader<noOrganizationLandingPage_rootQuery>(NoOrganizationLandingPageRootQuery);
  const { user, loading } = useAuth();

  useEffect(() => {
    if (loading || !user) {
      return;
    }

    loadQuery(
      {},
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, loading, user]);

  return (
    <NoOrganizationRootShell collapsed hideSideNav hideOrganizationSelector hideWelcomeMessage={!user}>
      {loading && (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: { xs: 2, md: 4 } }}>
          <CircularProgress />
        </Box>
      )}

      {!loading && !user && (
        <StackColumn sx={{ p: { xs: 2, md: 4 }, maxWidth: 760 }}>
          <Card variant="outlined">
            <CardContent>
              <StackColumn>
                <LeadIconTypography label="Select a private organisation" />
                <BodyIconTypography label="Teams is for private organisations, team membership, users, bookings, locations, and internal availability workflows." />
                <Button href={getSignInLink()} variant="contained" sx={{ alignSelf: 'flex-start', textTransform: 'none' }}>
                  Sign in
                </Button>
              </StackColumn>
            </CardContent>
          </Card>
        </StackColumn>
      )}

      {!loading && user && (
        <ErrorBoundary
          fallbackRender={({ error }) => {
            console.warn('org_landing_query_failed', {
              event: 'org_landing_query_failed',
              message: error instanceof Error ? error.message : String(error),
            });

            return <RelayError error={toRootError(error)} />;
          }}
        >
          <Suspense
            fallback={
              <Box sx={{ display: 'flex', justifyContent: 'center', p: { xs: 2, md: 4 } }}>
                <CircularProgress />
              </Box>
            }
          >
            {queryRef ? <NoOrganizationLandingContent queryRef={queryRef} /> : <CircularProgress />}
          </Suspense>
        </ErrorBoundary>
      )}
    </NoOrganizationRootShell>
  );
};

export default memo(RootPage);
