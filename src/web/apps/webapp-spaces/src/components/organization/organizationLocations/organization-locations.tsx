import { RelayError, startOfDay, toRootError } from '@skedular/shared';
import { Loading } from '@/components/loading';
import { NewLocationButton } from '@/components/location/addLocation';
import { CustomTagSelector } from '@/components/organization/customTagSelector';
import { ZoneSelector } from '@/components/organization/zoneSelector';

import type { organizationLocations_locations_availableOrganizationResources_query$key } from '@/queries/__generated__/organizationLocations_locations_availableOrganizationResources_query.graphql';
import type { organizationLocations_locations_availableOrganizationResources_refetchableFragment } from '@/queries/__generated__/organizationLocations_locations_availableOrganizationResources_refetchableFragment.graphql';
import type { organizationLocations_rootQuery } from '@/queries/__generated__/organizationLocations_rootQuery.graphql';
import Switch from '@mui/material/Switch';
import TextField from '@mui/material/TextField';
import Box from '@mui/system/Box';

import { BodyIconTypography, GridContainer, StackColumn, StackRow } from '@skedular/ui';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, startTransition, useCallback, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';
import { v7 as uuid } from 'uuid';
import LocationCard from './location-card';
import OrganizationLocationsPageShell from './organization-locations-page-shell';

type Props = {
  queryReference: PreloadedQuery<organizationLocations_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
};

const RootQuery = graphql`
  query organizationLocations_rootQuery(
    $organizationCustomDomain: String!
    $locationsSortingValues: [LocationOrderInput!]
    $zonesSortingValues: [OrganizationTagOrderInput!]
    $customTagsSortingValues: [OrganizationTagOrderInput!]
    $fromTodayDate: DateTime!
    $untilTodayDate: DateTime!
    $zoneIds: [String!]
    $customTagIds: [String!]
    $locationNotContactedYet: Boolean!
  ) {
    organization(customDomain: $organizationCustomDomain) {
      canModify
      customDomain
    }
    ...newLocationButton_query
    ...locationCard_query
    ...customTagSelector_allCustomTags_query
    ...zoneSelector_allZones_query
    ...organizationLocations_locations_availableOrganizationResources_query
  }
`;

const OrganizationLocations = ({ queryReference, onReloadRequired, organizationCustomDomain }: Props) => {
  const rootData = usePreloadedQuery<organizationLocations_rootQuery>(RootQuery, queryReference);
  const router = useRouter();
  const searchParams = useSearchParams();
  const customTagId = searchParams.get('customTagId');
  const zoneId = searchParams.get('zoneId');
  const customTagIds = useMemo(() => (customTagId ? [customTagId] : []), [customTagId]);
  const zoneIds = useMemo(() => (zoneId ? [zoneId] : []), [zoneId]);
  const hasParam = (name: string, fallback = false) => searchParams.get(name) === 'true' || (searchParams.get(name) === null && fallback);
  const locationNotContactedYet = hasParam('locationNotContactedYet');
  const filterThoseWithoutCoordites = hasParam('withoutCoordinates', organizationCustomDomain === 'skedularpubliclocations');
  const filterThoseWithCoordites = hasParam('withCoordinates');
  const filterThoseWithEmails = hasParam('withEmails');
  const filterThoseWithPhones = hasParam('withPhones');
  const phoneStartWith = searchParams.get('phoneStartsWith') ?? '';
  const [rootDataRefetchable, refetch] = useRefetchableFragment<
    organizationLocations_locations_availableOrganizationResources_refetchableFragment,
    organizationLocations_locations_availableOrganizationResources_query$key
  >(
    graphql`
      fragment organizationLocations_locations_availableOrganizationResources_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationLocations_locations_availableOrganizationResources_refetchableFragment") {
        locations(
          first: $count
          after: $cursor
          where: { organizationCustomDomain: $organizationCustomDomain, zoneIds: $zoneIds, customTagIds: $customTagIds, notContactedYet: $locationNotContactedYet }
          orderBy: $locationsSortingValues
        ) @connection(key: "organizationLocations_locations") {
          __id
          totalCount
          edges {
            node {
              id
              resources {
                totalCount
              }
              physicalAddress {
                longitude
                latitude
              }
              extraMetadata {
                contactDetails {
                  contactEmails
                  contactPhones
                }
              }
              ...locationCard_LocationDetails
            }
          }
        }
        availableResources(
          where: { organizationCustomDomain: $organizationCustomDomain, from: $fromTodayDate, until: $untilTodayDate, zoneIds: $zoneIds, customTagIds: $customTagIds }
        ) {
          location {
            uniqueId
          }
        }
      }
    `,
    rootData,
  );

  const [defaultDate] = useState(startOfDay());
  const connectionIds = useMemo(() => [rootDataRefetchable.locations.__id], [rootDataRefetchable.locations]);
  const updateFilterUrl = (updates: Record<string, string | boolean | undefined>) => {
    const params = new URLSearchParams(window.location.search);
    Object.entries(updates).forEach(([key, value]) => {
      if (value === true || (typeof value === 'string' && value)) params.set(key, String(value));
      else params.delete(key);
    });
    router.push(`?${params.toString()}`);
  };

  const locations = useMemo(
    () =>
      rootDataRefetchable.locations.edges
        .map((edge) => edge.node)
        .filter((item) => !filterThoseWithoutCoordites || !item.physicalAddress?.latitude || !item.physicalAddress?.longitude)
        .filter((item) => !filterThoseWithCoordites || (item.physicalAddress?.latitude && item.physicalAddress?.longitude))
        .filter((item) => !filterThoseWithEmails || item.extraMetadata?.contactDetails?.contactEmails?.length !== 0)
        .filter((item) => !filterThoseWithPhones || item.extraMetadata?.contactDetails?.contactPhones?.length !== 0)
        .filter(
          (item) =>
            !phoneStartWith ||
            item.extraMetadata?.contactDetails?.contactPhones?.some((phone) => {
              const sanitizedFilter = phoneStartWith.replace(/[^\d+]/g, '');
              const sanitizedPhone = (phone ?? '').replace(/[^\d+]/g, '');

              return sanitizedPhone.startsWith(sanitizedFilter);
            }),
        ),
    [rootDataRefetchable.locations, filterThoseWithoutCoordites, filterThoseWithCoordites, filterThoseWithEmails, filterThoseWithPhones, phoneStartWith],
  );
  const handleRefetch = useCallback(
    (customTagIds: string[], zoneIds: string[], locationNotContactedYet: boolean) => {
      startTransition(() => {
        refetch(
          {
            customTagIds,
            zoneIds,
            locationNotContactedYet,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetch],
  );

  useEffect(() => handleRefetch(customTagIds, zoneIds, locationNotContactedYet), [handleRefetch, customTagIds, zoneIds, locationNotContactedYet]);

  const handleCustomTagChanged = (id?: string) => {
    updateFilterUrl({ customTagId: id });
  };

  const handleZoneTypeChanged = (id?: string) => {
    updateFilterUrl({ zoneId: id });
  };

  const handleFilterThoseWithoutCoorditesChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    updateFilterUrl({ withoutCoordinates: event.target.checked });
  };

  const handleFilterThoseWithCoorditesChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    updateFilterUrl({ withCoordinates: event.target.checked });
  };

  const handleFilterThoseWithEmailsChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    updateFilterUrl({ withEmails: event.target.checked });
  };

  const handleFilterThoseWithPhonesChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    updateFilterUrl({ withPhones: event.target.checked });
  };

  const handleLocationNotContactedYetChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    updateFilterUrl({ locationNotContactedYet: event.target.checked });
  };

  const handlePhoneFilterChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    updateFilterUrl({ phoneStartsWith: event.target.value });
  };

  if (!rootDataRefetchable.locations || !rootDataRefetchable.availableResources || !rootData.organization) {
    return null;
  }

  const pageActions = <>{rootData.organization?.canModify && <NewLocationButton rootDataRelay={rootData} organizationCustomDomain={organizationCustomDomain} />}</>;

  const pageToolbar = (
    <StackColumn spacing={1.5}>
      <GridContainer spacing={1} sx={{ alignItems: 'center' }}>
        <ZoneSelector key={`zone-${zoneId ?? 'all'}`} rootDataRelay={rootData} onChange={handleZoneTypeChanged} defaultValue={zoneId} />
        <CustomTagSelector key={`tag-${customTagId ?? 'all'}`} rootDataRelay={rootData} onChange={handleCustomTagChanged} defaultValue={customTagId} />
      </GridContainer>

      {organizationCustomDomain === 'skedularpubliclocations' && (
        <StackRow sx={{ gap: 1.5, flexWrap: 'wrap', alignItems: 'center' }}>
          <BodyIconTypography label="Filter those without address" />
          <Switch checked={filterThoseWithoutCoordites} onChange={handleFilterThoseWithoutCoorditesChange} />

          <BodyIconTypography label="Filter those with address" />
          <Switch checked={filterThoseWithCoordites} onChange={handleFilterThoseWithCoorditesChange} />

          <BodyIconTypography label="Filter those with emails" />
          <Switch checked={filterThoseWithEmails} onChange={handleFilterThoseWithEmailsChange} />

          <BodyIconTypography label="Filter those with phones" />
          <Switch checked={filterThoseWithPhones} onChange={handleFilterThoseWithPhonesChange} />

          <BodyIconTypography label="Filter those not contacted yet" />
          <Switch checked={locationNotContactedYet} onChange={handleLocationNotContactedYetChange} />

          <BodyIconTypography label="Phone starts with" />
          <TextField value={phoneStartWith} onChange={handlePhoneFilterChange} />
        </StackRow>
      )}
    </StackColumn>
  );

  return (
    <OrganizationLocationsPageShell actions={pageActions} toolbar={pageToolbar} isEmpty={locations.length === 0}>
      <Box
        sx={{
          display: 'grid',
          gridTemplateColumns: {
            xs: '1fr',
            sm: 'repeat(auto-fit, minmax(320px, 360px))',
          },
          gap: 2,
          alignItems: 'stretch',
          justifyContent: 'start',
        }}
      >
        {locations.map((location) => {
          const resourcesCount = location.resources.totalCount;
          const availableResourcesCount = rootDataRefetchable.availableResources
            ? rootDataRefetchable.availableResources.filter((resources) => resources.location?.uniqueId === location.id).length
            : 0;
          const availablePercentage = resourcesCount > 0 ? (availableResourcesCount / resourcesCount) * 100 : 0;

          return (
            <LocationCard
              key={location.id}
              rootDataRelay={rootData}
              locationDetailsRelay={location}
              onReloadRequired={onReloadRequired}
              organizationCustomDomain={organizationCustomDomain}
              defaultDate={defaultDate}
              connectionIds={connectionIds}
              availableResourcesCount={availableResourcesCount}
              availablePercentage={availablePercentage}
            />
          );
        })}
      </Box>
    </OrganizationLocationsPageShell>
  );
};

const MemoOrganizationLocations = memo(OrganizationLocations);

type RelayProps = {
  organizationCustomDomain: string;
};

const OrganizationLocationsWithRelay = ({ organizationCustomDomain }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationLocations_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    const today = startOfDay();

    loadQuery(
      {
        organizationCustomDomain,
        locationsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
        zonesSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
        customTagsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
        fromTodayDate: today.toISOString(),
        untilTodayDate: today.add(1, 'day').toISOString(),
        locationNotContactedYet: false,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationCustomDomain]);

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
      <MemoOrganizationLocations queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationCustomDomain={organizationCustomDomain} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationLocationsWithRelay);
