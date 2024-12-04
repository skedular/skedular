import { LocationSelector } from '@/components/location/locationSelector';
import { NewTeamButton } from '@/components/team/addTeam';
import { MyTeams } from '@/components/team/myTeams';
import type { teams_rootQuery } from '@/queries/__generated__/teams_rootQuery.graphql';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import { ListGridToggle } from '@repo/shared/components/listGridToggle';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { defaultPadding } from '@repo/shared/libs/theme';
import { nanoid } from 'nanoid';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<teams_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query teams_rootQuery($organizationId: String!, $primaryLocationIds: [String!], $teamsSortingValues: [TeamOrderInput!]!) {
    ...locationSelector_allLocations_query
    ...myTeams_teams_availableOrganizationDesks_query
  }
`;

const Teams = ({ queryReference, onReloadRequired, organizationId }: Props) => {
  const rootData = usePreloadedQuery<teams_rootQuery>(RootQuery, queryReference);
  const [locationIds, setLocationIds] = useState<string[]>([]);
  const [viewMode, setViewMode] = useState<'list' | 'grid'>('grid');

  const handlLocationChanged = (id?: string) => {
    setLocationIds(id ? [id] : []);
  };

  const handlViewModeChanged = (newViewMode: 'list' | 'grid') => {
    setViewMode(newViewMode);
  };

  return (
    <Stack direction="column" spacing={1}>
      <Stack
        direction="row"
        spacing={1}
        sx={{
          alignItems: 'center',
          flexWrap: 'wrap',
          paddingLeft: defaultPadding,
          paddingRight: defaultPadding,
          paddingBottom: defaultPadding,
          paddingTop: defaultPadding,
        }}
      >
        <LocationSelector rootDataRelay={rootData} onChange={handlLocationChanged} />
        <ListGridToggle defaultValue={viewMode} onChange={handlViewModeChanged} />
        <Box sx={{ flexGrow: 1 }} /> {/* This will push NewBookingButton to the right */}
        <NewTeamButton organizationId={organizationId} />
      </Stack>
      <MyTeams rootDataRelay={rootData} onReloadRequired={onReloadRequired} primaryLocationIds={locationIds} viewMode={viewMode} />
    </Stack>
  );
};

const MemoTeams = memo(Teams);

type RelayProps = {
  organizationId: string;
};

const TeamsWithRelay = ({ organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<teams_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId,
        teamsSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationId]);

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
      <MemoTeams queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(TeamsWithRelay);
