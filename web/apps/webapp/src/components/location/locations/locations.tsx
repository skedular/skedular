import { MyLocations } from '@/components/location/myLocations';
import { ZoneSelector } from '@/components/location/zoneSelector';
import { DeskTypeSelector } from '@/components/organization/deskTypeSelector';
import type { locations_rootQuery } from '@/queries/__generated__/locations_rootQuery.graphql';
import Stack from '@mui/material/Stack';
import { ORGANIZATION_TAG_TYPE_DESK_TYPE } from '@repo/shared/components/deskType';
import { ListGridToggle } from '@repo/shared/components/listGridToggle';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { defaultPadding } from '@repo/shared/libs/theme';
import { startOfDay } from '@repo/shared/libs/utils';
import { nanoid } from 'nanoid';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<locations_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId?: string;
};

const RootQuery = graphql`
  query locations_rootQuery(
    $organizationId: String!
    $locationsSortingValues: [LocationOrderInput!]!
    $deskTypeTagType: String!
    $todayDate: DateTime!
    $organizationMembersSortingValues: [OrganizationMemberOrderInput!]
    $zoneIds: [String!]!
    $deskTypeIds: [String!]!
  ) {
    ...deskTypeSelector_allDeskTypes_query
    ...zoneSelector_allZones_query
    ...myLocations_query
    ...myLocations_locations_availableOrganizationDesks_query
  }
`;

const Locations = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<locations_rootQuery>(RootQuery, queryReference);
  const [deskTypeIds, setDeskTypeIds] = useState<string[]>([]);
  const [zoneIds, setZoneIds] = useState<string[]>([]);
  const [viewMode, setViewMode] = useState<'list' | 'grid'>('grid');

  const handleDeskTypeChanged = (id?: string) => {
    setDeskTypeIds(id ? [id] : []);
  };

  const handleZoneTypeChanged = (id?: string) => {
    setZoneIds(id ? [id] : []);
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
        <DeskTypeSelector rootDataRelay={rootData} onChange={handleDeskTypeChanged} />
        <ZoneSelector rootDataRelay={rootData} onChange={handleZoneTypeChanged} />
        <ListGridToggle defaultValue={viewMode} onChange={handlViewModeChanged} />
      </Stack>
      <MyLocations
        rootDataRelay={rootData}
        rootDataRefetchableRelay={rootData}
        onReloadRequired={onReloadRequired}
        deskTypeIds={deskTypeIds}
        zoneIds={zoneIds}
        viewMode={viewMode}
      />
    </Stack>
  );
};

const MemoLocations = memo(Locations);

type RelayProps = {
  organizationId?: string;
};

const LocationsWithRelay = ({ organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<locations_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    const today = startOfDay();

    loadQuery(
      {
        organizationId: organizationId ?? '',
        deskTypeTagType: ORGANIZATION_TAG_TYPE_DESK_TYPE,
        locationsSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        todayDate: today.toISOString(),
        organizationMembersSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        deskTypeIds: [],
        zoneIds: [],
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
      <MemoLocations queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(LocationsWithRelay);
