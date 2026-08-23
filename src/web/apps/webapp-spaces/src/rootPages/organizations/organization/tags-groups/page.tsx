import { RelayError, toRootError, useKnownParams } from '@skedular/shared';
import { Loading } from '@/components/loading';
import { OrganizationAdmin } from '@/components/organization/organizationAdmin';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationTagsGroups_rootQuery } from '@/queries/__generated__/pageOrganizationTagsGroups_rootQuery.graphql';
import { memo, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageOrganizationTagsGroups_rootQuery, Record<string, unknown>>;
  organizationCustomDomain: string;
};

const RootQuery = graphql`
  query pageOrganizationTagsGroups_rootQuery($organizationCustomDomain: String!) {
    organization(customDomain: $organizationCustomDomain) {
      name
    }
    ...organizationAdmin_query
  }
`;

const RootPage = ({ queryReference, organizationCustomDomain }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationTagsGroups_rootQuery>(RootQuery, queryReference);
  return (
    <RootShell>
      <OrganizationAdmin rootDataRelay={rootData} organizationCustomDomain={organizationCustomDomain} tagsGroupsMode />
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationTagsGroups_rootQuery>(RootQuery);
  const { organizationCustomDomain } = useKnownParams();
  if (!organizationCustomDomain) throw new Error('organizationCustomDomain is required');
  useEffect(() => {
    loadQuery({ organizationCustomDomain }, { fetchPolicy: 'store-and-network' });
  }, [loadQuery, organizationCustomDomain]);
  if (!queryReference) return <Loading />;
  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoRootPage queryReference={queryReference} organizationCustomDomain={organizationCustomDomain} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
