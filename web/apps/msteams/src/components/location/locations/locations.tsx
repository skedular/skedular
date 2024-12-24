import { PushToRight, StackColumn, StackRow } from '@repo/shared/components/commons';
import { ListGridToggle } from '@repo/shared/components/listGridToggle';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { defaultPadding, maxScreenWidth } from '@repo/shared/libs/theme';
import { startOfDay } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { NewLocationButton } from 'components/location/addLocation';
import { MyLocations } from 'components/location/myLocations';
import { DeskTypeSelector } from 'components/organization/deskTypeSelector';
import { ZoneSelector } from 'components/organization/zoneSelector';
import { nanoid } from 'nanoid';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { locations_rootQuery } from './__generated__/locations_rootQuery.graphql';

type Props = {
  queryReference: PreloadedQuery<locations_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query locations_rootQuery(
    $organizationId: String!
    $locationsSortingValues: [LocationOrderInput!]!
    $zonesSortingValues: [OrganizationTagOrderInput!]!
    $todayDate: DateTime!
    $organizationMembersSortingValues: [OrganizationMemberOrderInput!]
    $zoneIds: [String!]!
    $deskTypeIds: [String!]!
  ) {
    organization(id: $organizationId) {
      canModify
    }
    ...deskTypeSelector_allDeskTypes_query
    ...zoneSelector_allZones_query
    ...myLocations_query
    ...myLocations_locations_availableOrganizationDesks_query
  }
`;

const Locations = ({ queryReference, onReloadRequired, organizationId }: Props) => {
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
    <StackColumn sx={{ maxWidth: maxScreenWidth }}>
      <StackRow sx={{ padding: defaultPadding }}>
        <DeskTypeSelector rootDataRelay={rootData} onChange={handleDeskTypeChanged} />
        <ZoneSelector rootDataRelay={rootData} onChange={handleZoneTypeChanged} />
        <ListGridToggle defaultValue={viewMode} onChange={handlViewModeChanged} />
        <PushToRight />
        {rootData.organization?.canModify && <NewLocationButton organizationId={organizationId} />}
      </StackRow>
      <MyLocations
        rootDataRelay={rootData}
        rootDataRefetchableRelay={rootData}
        onReloadRequired={onReloadRequired}
        organizationId={organizationId}
        deskTypeIds={deskTypeIds}
        zoneIds={zoneIds}
        viewMode={viewMode}
      />
    </StackColumn>
  );
};

const MemoLocations = memo(Locations);

type RelayProps = {
  organizationId: string;
};

const LocationsWithRelay = ({ organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<locations_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    const today = startOfDay();

    loadQuery(
      {
        organizationId,
        locationsSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        zonesSortingValues: [
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
      <MemoLocations queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(LocationsWithRelay);
