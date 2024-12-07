import { CustomerDaySummary } from '@/components/customer/customerDaySummary';
import { LocationBookingsCard } from '@/components/location/locationBookingCard';
import { TeamBookingsCard } from '@/components/team/teamBookingCard';
import type { dashboard_rootQuery } from '@/queries/__generated__/dashboard_rootQuery.graphql';
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid2';
import Stack from '@mui/material/Stack';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { startOfDay } from '@repo/shared/libs/utils';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<dashboard_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId?: string;
};

const RootQuery = graphql`
  query dashboard_rootQuery($organizationId: String!, $nullableOrganizationId: String, $organizationExists: Boolean!) {
    organization(id: $organizationId) @include(if: $organizationExists) {
      id
      name
    }
    myLocations(organizationId: $nullableOrganizationId) {
      id
      name
      organization {
        uniqueId
        name
      }
    }
    myTeams(organizationId: $nullableOrganizationId) {
      id
      name
      organization {
        uniqueId
        name
      }
    }
  }
`;

const Dashboard = ({ queryReference, organizationId }: Props) => {
  const rootData = usePreloadedQuery<dashboard_rootQuery>(RootQuery, queryReference);
  const [today] = useState(startOfDay());
  const [tomorrow] = useState(startOfDay().add(1, 'day'));

  if (!rootData.myTeams || !rootData.myLocations) {
    return <></>;
  }

  return (
    <>
      <Box sx={{ display: { xs: 'none', sm: 'none', md: 'none', lg: 'block', xl: 'block' } }}>
        <Stack direction="row" spacing={1}>
          <Stack direction="column" spacing={1}>
            <CustomerDaySummary date={today} minWidth={300} organizationId={organizationId} />
            <CustomerDaySummary date={tomorrow} minWidth={300} organizationId={organizationId} />
          </Stack>
          <Grid container spacing={2}>
            {rootData.myLocations.map((location) => (
              <Grid key={location.id}>
                <LocationBookingsCard
                  organizationId={location.organization?.uniqueId}
                  organizationName={location.organization?.name}
                  locationId={location.id}
                  locationName={location.name}
                  locationsConnectionIds={[]}
                  hideRemoveLocationOption
                />
              </Grid>
            ))}
            {rootData.myTeams.map((team) => (
              <Grid key={team.id}>
                <TeamBookingsCard
                  organizationId={team.organization?.uniqueId}
                  organizationName={team.organization?.name}
                  teamId={team.id}
                  teamName={team.name}
                  teamsConnectionIds={[]}
                  hideRemoveTeamOption
                />
              </Grid>
            ))}
          </Grid>
        </Stack>
      </Box>
      <Box sx={{ display: { xs: 'block', sm: 'block', md: 'block', lg: 'none', xl: 'none' } }}>
        <Grid>
          <CustomerDaySummary date={today} />
        </Grid>
        <Grid>
          <CustomerDaySummary date={tomorrow} />
        </Grid>
        {rootData.myLocations.map((location) => (
          <Grid key={location.id}>
            <LocationBookingsCard
              organizationId={location.organization?.uniqueId}
              organizationName={location.organization?.name}
              locationId={location.id}
              locationName={location.name}
              locationsConnectionIds={[]}
              hideRemoveLocationOption
            />
          </Grid>
        ))}
        {rootData.myTeams.map((team) => (
          <Grid key={team.id}>
            <TeamBookingsCard
              organizationId={team.organization?.uniqueId}
              organizationName={team.organization?.name}
              teamId={team.id}
              teamName={team.name}
              teamsConnectionIds={[]}
              hideRemoveTeamOption
            />
          </Grid>
        ))}
      </Box>
    </>
  );
};

const MemoDashboard = memo(Dashboard);

type RelayProps = {
  organizationId?: string;
};

const DashboardWithRelay = ({ organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<dashboard_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId: organizationId ?? '',
        nullableOrganizationId: organizationId,
        organizationExists: !!organizationId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReload, organizationId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReload(triggerReload + 1);
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoDashboard queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(DashboardWithRelay);
