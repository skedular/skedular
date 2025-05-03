'use client';

import { BodyIconTypography, StackColumn } from '@/components/commons';
import { Loading } from '@/components/loading';
import { OrganizationTeam } from '@/components/organization/organizationTeam';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationTeam_rootQuery } from '@/queries/__generated__/pageOrganizationTeam_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { nanoid } from 'nanoid';
import { useParams, useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';

const RootQuery = graphql`
  query pageOrganizationTeam_rootQuery($organizationId: String!, $organizationExists: Boolean!, $teamId: String!, $peopleNameSearchText: String) {
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
  organizationId: string;
  teamId: string;
};

const RootPage = ({ queryReference, onReloadRequired, organizationId, teamId }: Props) => {
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
      <OrganizationTeam rootDataRelay={rootData} rootDataTeamMembersRelay={rootData} onReloadRequired={onReloadRequired} organizationId={organizationId} teamId={teamId} />
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationTeam_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();
  const { organizationId, teamId } = useParams();
  let finalOrganizationId = '';

  if (typeof organizationId === 'string') {
    finalOrganizationId = organizationId;
  } else if (Array.isArray(organizationId)) {
    if (typeof organizationId[0] === 'undefined') {
      throw new Error('organizationId is required');
    }

    finalOrganizationId = organizationId[0];
  } else {
    throw new Error('organizationId is required');
  }

  let finalTeamId = '';

  if (typeof teamId === 'string') {
    finalTeamId = teamId;
  } else if (Array.isArray(teamId)) {
    if (typeof teamId[0] === 'undefined') {
      throw new Error('teamId is required');
    }

    finalTeamId = teamId[0];
  } else {
    throw new Error('teamId is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationId: finalOrganizationId,
        organizationExists: !!finalOrganizationId,
        teamId: finalTeamId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, finalOrganizationId, finalTeamId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(nanoid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={finalOrganizationId} teamId={finalTeamId} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
