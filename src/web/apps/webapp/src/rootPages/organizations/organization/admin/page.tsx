import { Loading } from '@/components/loading';
import { OrganizationAdmin } from '@/components/organization/organizationAdmin';
import { RelayError, toRootError } from '@skedular/shared';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationAdmin_rootQuery } from '@/queries/__generated__/pageOrganizationAdmin_rootQuery.graphql';
import { memo, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import useKnownParams from '@/hooks/use-known-params';

type Props = {
  queryReference: PreloadedQuery<pageOrganizationAdmin_rootQuery, Record<string, unknown>>;
  organizationCustomDomain: string;
};

const RootQuery = graphql`
  query pageOrganizationAdmin_rootQuery($organizationCustomDomain: String!) {
    organization(customDomain: $organizationCustomDomain) {
      name
    }
    ...organizationAdmin_query
  }
`;

const RootPage = ({ queryReference, organizationCustomDomain }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationAdmin_rootQuery>(RootQuery, queryReference);

  return (
    <RootShell>
      <OrganizationAdmin rootDataRelay={rootData} organizationCustomDomain={organizationCustomDomain} />
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationAdmin_rootQuery>(RootQuery);
  const { organizationCustomDomain } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, organizationCustomDomain]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoRootPage queryReference={queryReference} organizationCustomDomain={organizationCustomDomain} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
