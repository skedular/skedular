import { Loading } from '@/components/loading';
import { OrganizationTeam } from '@/components/organization/organizationTeam';
import { RelayError, toRootError } from '@skedular/shared';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationTeam_rootQuery } from '@/queries/__generated__/pageOrganizationTeam_rootQuery.graphql';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';
import useKnownParams from '@/hooks/use-known-params';

const RootQuery = graphql`
  query pageOrganizationTeam_rootQuery($organizationCustomDomain: String!, $teamId: String!, $peopleNameSearchText: String) {
    team(id: $teamId) {
      name
    }
    ...organizationTeam_query
    ...organizationTeam_teamMembers_query
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageOrganizationTeam_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  teamId: string;
};

const RootPage = ({ queryReference, onReloadRequired, organizationCustomDomain, teamId }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationTeam_rootQuery>(RootQuery, queryReference);

  return (
    <RootShell>
      <OrganizationTeam
        rootDataRelay={rootData}
        rootDataTeamMembersRelay={rootData}
        onReloadRequired={onReloadRequired}
        organizationCustomDomain={organizationCustomDomain}
        teamId={teamId}
      />
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationTeam_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { organizationCustomDomain, teamId } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  if (!teamId) {
    throw new Error('teamId is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
        teamId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationCustomDomain, teamId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationCustomDomain={organizationCustomDomain} teamId={teamId} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
