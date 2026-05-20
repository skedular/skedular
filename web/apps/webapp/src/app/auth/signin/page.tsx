'use client';

import CustomOrganizationAuthPage from '@/components/auth/custom-organization-auth-page';
import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@/components/relayError';
import type { pageAuthSignInQuery } from '@/queries/__generated__/pageAuthSignInQuery.graphql';
import { getOrganizationCustomDomainFromHost } from '../host-utils';
import { useSearchParams } from 'next/navigation';
import { memo, Suspense, useEffect, useSyncExternalStore } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageAuthSignInQuery>;
  returnTo?: string;
  error?: string | null;
};

const RootQuery = graphql`
  query pageAuthSignInQuery($organizationCustomDomain: String!) {
    organizationPublic(customDomain: $organizationCustomDomain) {
      name
      logoUrl
      featureImages {
        original {
          url
        }
        thumbnail {
          url
        }
      }
    }
  }
`;

const AuthSignInPage = ({ queryReference, returnTo, error }: Props) => {
  const rootData = usePreloadedQuery<pageAuthSignInQuery>(RootQuery, queryReference);

  return (
    <CustomOrganizationAuthPage
      mode="sign-in"
      organizationName={rootData.organizationPublic?.name}
      organizationLogoUrl={rootData.organizationPublic?.logoUrl}
      organizationFeatureImageUrl={rootData.organizationPublic?.featureImages[0]?.original?.url ?? rootData.organizationPublic?.featureImages[0]?.thumbnail?.url}
      returnTo={returnTo}
      error={error}
    />
  );
};

const MemoAuthSignInPage = memo(AuthSignInPage);

const subscribeToHostname = () => () => undefined;
const getServerCustomDomainOrganizationName = () => null;

const getCustomDomainOrganizationName = () => {
  return getOrganizationCustomDomainFromHost(window.location.hostname) ?? '';
};

const AuthSignInPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageAuthSignInQuery>(RootQuery);
  const organizationCustomDomain = useSyncExternalStore(subscribeToHostname, getCustomDomainOrganizationName, getServerCustomDomainOrganizationName);
  const searchParams = useSearchParams();
  const rawReturnTo = searchParams.get('returnTo') ?? undefined;
  const returnTo = rawReturnTo?.startsWith('/') ? rawReturnTo : undefined;
  const error = searchParams.get('error');

  useEffect(() => {
    if (organizationCustomDomain === null) {
      return;
    }

    if (!organizationCustomDomain) {
      return;
    }

    loadQuery({ organizationCustomDomain }, { fetchPolicy: 'store-and-network' });
  }, [loadQuery, organizationCustomDomain]);

  if (organizationCustomDomain === null) {
    return <Loading />;
  }

  if (!organizationCustomDomain) {
    return <CustomOrganizationAuthPage mode="sign-in" returnTo={returnTo} error={error} />;
  }

  if (!queryReference) {
    return <Loading />;
  }

  return <MemoAuthSignInPage queryReference={queryReference} returnTo={returnTo} error={error} />;
};

const RootPage = () => (
  <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
    <Suspense fallback={<Loading />}>
      <AuthSignInPageWithRelay />
    </Suspense>
  </ErrorBoundary>
);

export default memo(RootPage);
