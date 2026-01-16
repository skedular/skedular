import { NewBookingButton } from '@/components/booking/addBooking';
import { MyBookings } from '@/components/booking/myBookings';
import { GridContainer, PushToRight, StackColumn } from '@/components/commons';
import { WeekRangePicker } from '@/components/datePickers';
import { GettingStarted } from '@/components/gettingStarted';
import { getOrganizationLocationsBaseLink } from '@/components/links';
import { ListGridToggle } from '@/components/listGridToggle';
import { Loading } from '@/components/loading';
import { ClaimLocationOwnershipButton } from '@/components/location';
import { LocationSelector } from '@/components/location/locationSelector';
import { RelayError, toRootError } from '@/components/relayError';
import { TeamSelector } from '@/components/team/teamSelector';
import { useIntegratedPlatrform } from '@/libs/providers';
import { defaultPadding, maxScreenWidth } from '@/libs/theme';
import { endOfWeek, startOfDay, startOfWeek } from '@/libs/utils';
import type { organization_rootQuery } from '@/queries/__generated__/organization_rootQuery.graphql';
import Box from '@mui/system/Box';
import { Dayjs } from 'dayjs';
import { useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<organization_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationUniqueAlphanumericName: string;
  defaultStartWeek: Dayjs;
};

const RootQuery = graphql`
  query organization_rootQuery(
    $organizationUniqueAlphanumericName: String!
    $locationIds: [String!]!
    $teamIds: [String!]!
    $bookingsSearchCriteriaFrom: DateTime!
    $bookingsSearchCriteriaTo: DateTime!
    $locationsSortingValues: [LocationOrderInput!]
  ) {
    organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {
      canModify
    }
    ...locationSelector_allLocations_query
    ...teamSelector_allTeams_query
    ...gettingStarted_query
    ...myBookings_query
    ...myBookings_bookings_query
  }
`;

const Organization = ({ queryReference, onReloadRequired, organizationUniqueAlphanumericName, defaultStartWeek }: Props) => {
  const rootData = usePreloadedQuery<organization_rootQuery>(RootQuery, queryReference);
  const { integratedPlatrform } = useIntegratedPlatrform();
  const [today] = useState(startOfDay());
  const [startWeek, setStartWeek] = useState(defaultStartWeek);
  const [endWeek, setEndWeek] = useState(endOfWeek(defaultStartWeek).add(-1, 'milliseconds'));
  const [locationIds, setLocationIds] = useState<string[]>([]);
  const [teamIds, setTeamIds] = useState<string[]>([]);
  const [viewMode, setViewMode] = useState<'list' | 'grid'>('grid');
  const router = useRouter();

  const handleWeehChanged = (date: Dayjs) => {
    setStartWeek(date);
    setEndWeek(endOfWeek(date).add(-1, 'milliseconds'));
  };

  const handlLocationChanged = (id?: string) => {
    setLocationIds(id ? [id] : []);
  };

  const handlTeamChanged = (id?: string) => {
    setTeamIds(id ? [id] : []);
  };

  const handlViewModeChanged = (newViewMode: 'list' | 'grid') => {
    setViewMode(newViewMode);
  };

  const handleClaimLocationOwnershipClicked = () => {
    router.push(getOrganizationLocationsBaseLink(integratedPlatrform, organizationUniqueAlphanumericName));
  };

  if (!organizationUniqueAlphanumericName) {
    return null;
  }

  return (
    <StackColumn sx={{ maxWidth: maxScreenWidth }}>
      <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
        <LocationSelector rootDataRelay={rootData} onChange={handlLocationChanged} />
        <TeamSelector rootDataRelay={rootData} onChange={handlTeamChanged} />
        <WeekRangePicker defaultStartWeek={startWeek} onWeekChanged={handleWeehChanged} />
        <ListGridToggle defaultValue={viewMode} onChange={handlViewModeChanged} />
        <PushToRight />
        <NewBookingButton onReloadRequired={onReloadRequired} defaultDate={today} organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} />
        {rootData.organization?.canModify && (
          <ClaimLocationOwnershipButton organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} onClaimClicked={handleClaimLocationOwnershipClicked} />
        )}
      </GridContainer>
      <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
        <GettingStarted rootDataRelay={rootData} onReloadRequired={onReloadRequired} organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} />
      </Box>
      <MyBookings
        rootDataRelay={rootData}
        rootDataBookingRelay={rootData}
        onReloadRequired={onReloadRequired}
        organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
        from={startWeek}
        to={endWeek}
        locationIds={locationIds}
        teamIds={teamIds}
        viewMode={viewMode}
      />
    </StackColumn>
  );
};

const MemoOrganization = memo(Organization);

type RelayProps = {
  organizationUniqueAlphanumericName: string;
};

const OrganizationWithRelay = ({ organizationUniqueAlphanumericName }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organization_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);
  const [, startTransition] = useTransition();
  const [startWeek] = useState(startOfWeek());

  useEffect(() => {
    const bookingsSearchCriteriaFrom = startWeek.toISOString();
    const bookingsSearchCriteriaTo = endOfWeek(startWeek).add(-1, 'milliseconds').toISOString();

    loadQuery(
      {
        organizationUniqueAlphanumericName,
        bookingsSearchCriteriaFrom,
        bookingsSearchCriteriaTo,
        locationIds: [],
        teamIds: [],
        locationsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReload, startWeek, organizationUniqueAlphanumericName]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReload(triggerReload + 1);
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoOrganization
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
        defaultStartWeek={startWeek}
      />
    </ErrorBoundary>
  );
};

export default memo(OrganizationWithRelay);
