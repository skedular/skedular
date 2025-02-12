import { NewBookingButton } from '@/components/booking/addBooking';
import { MyBookings } from '@/components/booking/myBookings';
import { GridContainer, PushToRight, StackColumn } from '@/components/commons';
import { WeekRangePicker } from '@/components/datePickers';
import { GettingStarted } from '@/components/gettingStarted';
import { ListGridToggle } from '@/components/listGridToggle';
import { Loading } from '@/components/loading';
import { LocationSelector } from '@/components/location/locationSelector';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { TeamSelector } from '@/components/team/teamSelector';
import { defaultPadding, maxScreenWidth } from '@/libs/theme';
import { endOfWeek, startOfDay, startOfWeek } from '@/libs/utils';
import type { organization_rootQuery } from '@/queries/__generated__/organization_rootQuery.graphql';
import { Dayjs } from 'dayjs';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<organization_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  defaultStartWeek: Dayjs;
};

const RootQuery = graphql`
  query organization_rootQuery(
    $organizationId: String!
    $nullableOrganizationId: String
    $locationIds: [String!]!
    $teamIds: [String!]!
    $bookingsSearchCriteriaFrom: DateTime!
    $bookingsSearchCriteriaTo: DateTime!
    $locationsSortingValues: [LocationOrderInput!]
  ) {
    organization(id: $organizationId) {
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
    ...locationSelector_allLocations_query
    ...teamSelector_allTeams_query
    ...gettingStarted_query
    ...myBookings_query
    ...myBookings_bookings_query
  }
`;

const Organization = ({ queryReference, onReloadRequired, organizationId, defaultStartWeek }: Props) => {
  const rootData = usePreloadedQuery<organization_rootQuery>(RootQuery, queryReference);
  const [today] = useState(startOfDay());
  const [startWeek, setStartWeek] = useState(defaultStartWeek);
  const [endWeek, setEndWeek] = useState(endOfWeek(defaultStartWeek).add(-1, 'milliseconds'));
  const [locationIds, setLocationIds] = useState<string[]>([]);
  const [teamIds, setTeamIds] = useState<string[]>([]);
  const [viewMode, setViewMode] = useState<'list' | 'grid'>('grid');

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

  if (!rootData.myTeams || !rootData.myLocations) {
    return <></>;
  }

  if (!organizationId) {
    return <></>;
  }

  return (
    <StackColumn sx={{ maxWidth: maxScreenWidth }}>
      <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
        <LocationSelector rootDataRelay={rootData} onChange={handlLocationChanged} />
        <TeamSelector rootDataRelay={rootData} onChange={handlTeamChanged} />
        <WeekRangePicker defaultStartWeek={startWeek} onWeekChanged={handleWeehChanged} />
        <ListGridToggle defaultValue={viewMode} onChange={handlViewModeChanged} />
        <PushToRight />
        <NewBookingButton onReloadRequired={onReloadRequired} defaultDate={today} organizationId={organizationId} />
      </GridContainer>
      <GettingStarted rootDataRelay={rootData} onReloadRequired={onReloadRequired} organizationId={organizationId} />
      <MyBookings
        rootDataRelay={rootData}
        rootDataBookingRelay={rootData}
        onReloadRequired={onReloadRequired}
        organizationId={organizationId}
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
  organizationId: string;
};

const OrganizationWithRelay = ({ organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organization_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);
  const [, startTransition] = useTransition();
  const [startWeek] = useState(startOfWeek());

  useEffect(() => {
    const bookingsSearchCriteriaFrom = startWeek.toISOString();
    const bookingsSearchCriteriaTo = endOfWeek(startWeek).add(-1, 'milliseconds').toISOString();

    loadQuery(
      {
        organizationId,
        nullableOrganizationId: organizationId,
        bookingsSearchCriteriaFrom,
        bookingsSearchCriteriaTo,
        locationIds: [],
        teamIds: [],
        locationsSortingValues: [
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
  }, [loadQuery, triggerReload, startWeek, organizationId]);

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
      <MemoOrganization queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} defaultStartWeek={startWeek} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationWithRelay);
