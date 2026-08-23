import { RelayError, toRootError, useKnownParams } from '@skedular/shared';
import { Loading } from '@/components/loading';
import OrganizationIntegration from '@/components/organization/organizationIntegration/organization-integration';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationIntegration_rootQuery } from '@/queries/__generated__/pageOrganizationIntegration_rootQuery.graphql';
import { memo, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';

const RootQuery = graphql`
  query pageOrganizationIntegration_rootQuery($organizationCustomDomain: String!) {
    organization(customDomain: $organizationCustomDomain) {
      name
    }
    ...organizationAdmin_query
  }
`;
const RootPage = ({
  queryReference,
  organizationCustomDomain,
}: {
  queryReference: PreloadedQuery<pageOrganizationIntegration_rootQuery, Record<string, unknown>>;
  organizationCustomDomain: string;
}) => {
  usePreloadedQuery<pageOrganizationIntegration_rootQuery>(RootQuery, queryReference);
  return (
    <RootShell>
      <OrganizationIntegration organizationCustomDomain={organizationCustomDomain} />
    </RootShell>
  );
};
const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationIntegration_rootQuery>(RootQuery);
  const { organizationCustomDomain } = useKnownParams();
  if (!organizationCustomDomain) throw new Error('organizationCustomDomain is required');
  useEffect(() => {
    loadQuery({ organizationCustomDomain }, { fetchPolicy: 'store-and-network' });
  }, [loadQuery, organizationCustomDomain]);
  if (!queryReference) return <Loading />;
  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <RootPage queryReference={queryReference} organizationCustomDomain={organizationCustomDomain} />
    </ErrorBoundary>
  );
};
export default memo(RootPageWithRelay);
