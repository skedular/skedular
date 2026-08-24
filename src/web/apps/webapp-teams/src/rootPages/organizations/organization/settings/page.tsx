import { RelayError, toRootError, useKnownParams } from '@skedular/shared';
import { Loading } from '@/components/loading';
import { OrganizationSettings } from '@/components/organization/organizationSettings';

import { RootShell } from '@/components/rootShell';

import type { pageOrganizationSettings_rootQuery } from '@/queries/__generated__/pageOrganizationSettings_rootQuery.graphql';
import { memo, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageOrganizationSettings_rootQuery, Record<string, unknown>>;
  organizationCustomDomain: string;
};

const RootQuery = graphql`
  query pageOrganizationSettings_rootQuery($organizationCustomDomain: String!) {
    organization(customDomain: $organizationCustomDomain) {
      name
    }
    ...organizationSettings_query
  }
`;

const RootPage = ({ queryReference, organizationCustomDomain }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationSettings_rootQuery>(RootQuery, queryReference);

  return (
    <RootShell>
      <OrganizationSettings rootDataRelay={rootData} organizationCustomDomain={organizationCustomDomain} />
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationSettings_rootQuery>(RootQuery);
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
