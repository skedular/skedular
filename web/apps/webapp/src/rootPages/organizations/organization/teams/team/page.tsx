import { BodyIconTypography, StackColumn } from '@/components/commons';
import { Loading } from '@/components/loading';
import { OrganizationTeam } from '@/components/organization/organizationTeam';
import { RelayError, toRootError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import { useKnownParams } from '@/libs/providers';
import type { pageOrganizationTeam_rootQuery } from '@/queries/__generated__/pageOrganizationTeam_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

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
  const router = useRouter();

  const handleBackClick = () => {
    router.back();
  };

  const breadcrumbs = (
    <StackColumn sx={{ alignItems: 'flex-start' }} spacing={0}>
      <Button variant="text" onClick={handleBackClick} sx={{ whiteSpace: 'nowrap', textTransform: 'none' }}>
        {'< back'}
      </Button>
      <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
        <Breadcrumbs>
          <BodyIconTypography label="Team Settings" />
          <BodyIconTypography label={rootData.team?.name} />
        </Breadcrumbs>
      </Box>
    </StackColumn>
  );

  return (
    <RootShell collapsed hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
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
