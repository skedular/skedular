import { getSignInLink, getSignUpLink } from '@/components/links';
import { NoOrganizationLandingContent, NoOrganizationLandingPageRootQuery } from '@/components/noOrganizationLanding';
import { RelayError, toRootError } from '@skedular/shared';
import { NoOrganizationRootShell, UnauthenticatedRootShell } from '@/components/rootShell';
import type { noOrganizationLandingPage_rootQuery } from '@/queries/__generated__/noOrganizationLandingPage_rootQuery.graphql';
import Box from '@mui/material/Box';
import CircularProgress from '@mui/material/CircularProgress';
import { ProductAuthenticationPrompt } from '@skedular/ui';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import { memo, Suspense, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { useQueryLoader } from 'react-relay';

const AuthenticatedRootPage = () => {
  const [queryRef, loadQuery] = useQueryLoader<noOrganizationLandingPage_rootQuery>(NoOrganizationLandingPageRootQuery);

  useEffect(() => {
    loadQuery(
      {},
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery]);

  return (
    <NoOrganizationRootShell hideSideNav hideOrganizationSelector>
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
    </NoOrganizationRootShell>
  );
};

const UnauthenticatedRootPage = () => (
  <UnauthenticatedRootShell>
    <ProductAuthenticationPrompt
      title="Welcome to Skedular Teams"
      description="To use Skedular Teams, create an account or sign in to your existing account."
      signInHref={getSignInLink()}
      signUpHref={getSignUpLink()}
    />
  </UnauthenticatedRootShell>
);

const RootPage = () => {
  const { user, loading } = useAuth();

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', p: { xs: 2, md: 4 } }}>
        <CircularProgress />
      </Box>
    );
  }

  return user ? <AuthenticatedRootPage /> : <UnauthenticatedRootPage />;
};

export default memo(RootPage);
