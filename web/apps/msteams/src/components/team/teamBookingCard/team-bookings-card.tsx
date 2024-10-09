import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Skeleton from '@mui/material/Skeleton';
import Stack from '@mui/material/Stack';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { endOfWeek, startOfWeek } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { OrganizationLink } from 'components/organization';
import { TeamLink } from 'components/team';
import { memo, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { teamBookingsCard_rootQuery } from './__generated__/teamBookingsCard_rootQuery.graphql';
import TeamPeopleBookings from './team-people-bookings';

type Props = {
  queryReference: PreloadedQuery<teamBookingsCard_rootQuery, Record<string, unknown>>;
  organizationId?: string;
  teamId: string;
  teamName?: string;
  teamsConnectionIds: string[];
  hideRemoveTeamOption?: boolean;
};

const RootQuery = graphql`
  query teamBookingsCard_rootQuery(
    $peopleNameSearchText: String
    $peopleSortingValues: [TeamMemberOrderInput!]!
    $teamId: String!
    $teamExists: Boolean!
    $from: DateTime!
    $to: DateTime!
  ) {
    ...teamPeopleBookings_query
  }
`;

const TeamBookingsCard = ({ queryReference, organizationId, teamId, teamName, teamsConnectionIds, hideRemoveTeamOption }: Props) => {
  const rootData = usePreloadedQuery<teamBookingsCard_rootQuery>(RootQuery, queryReference);

  return (
    <TeamPeopleBookings
      rootDataRelay={rootData}
      organizationId={organizationId}
      teamId={teamId}
      teamName={teamName}
      teamsConnectionIds={teamsConnectionIds}
      hideRemoveTeamOption={hideRemoveTeamOption}
    />
  );
};

const MemoTeamBookingsCard = memo(TeamBookingsCard);

type RelayProps = {
  organizationId?: string;
  organizationName?: string;
  teamId: string;
  teamName?: string;
  teamsConnectionIds: string[];
  hideRemoveTeamOption?: boolean;
};

const TeamBookingsWithRelay = ({ organizationId, organizationName, teamId, teamName, teamsConnectionIds, hideRemoveTeamOption }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<teamBookingsCard_rootQuery>(RootQuery);

  useEffect(() => {
    const startDate = startOfWeek();
    const endDate = endOfWeek(startDate);

    loadQuery(
      {
        peopleSortingValues: [
          {
            direction: 'Ascending',
            field: 'name',
          },
        ],
        teamId,
        teamExists: !!teamId,
        from: startDate.toISOString(),
        to: endDate.toISOString(),
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, teamId, organizationId]);

  if (!queryReference) {
    return (
      <Card sx={{ maxWidth: 500, height: '100%' }}>
        <CardHeader
          title={
            <Stack direction="column">
              <TeamLink organizationId={organizationId} id={teamId} name={teamName} />
              {organizationId && <OrganizationLink id={organizationId} name={organizationName} />}
            </Stack>
          }
        />
        <CardContent>
          <Skeleton variant="rounded" width={470} height={350} />
        </CardContent>
      </Card>
    );
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoTeamBookingsCard
        queryReference={queryReference}
        organizationId={organizationId}
        teamId={teamId}
        teamName={teamName}
        teamsConnectionIds={teamsConnectionIds}
        hideRemoveTeamOption={hideRemoveTeamOption}
      />
    </ErrorBoundary>
  );
};

export default memo(TeamBookingsWithRelay);
