import { RelayError, endOfWeek, startOfDay, startOfWeek, toRootError } from '@skedular/shared';
import { NewBookingButton } from '@/components/booking/addBooking';
import { MyBookings } from '@/components/booking/myBookings';
import { GridContainer, StackColumn } from '@skedular/ui';
import { WeekRangePicker } from '@/components/datePickers';
import { GettingStarted } from '@/components/gettingStarted';
import { Loading } from '@/components/loading';
import { LocationSelector } from '@/components/location/locationSelector';

import { TeamSelector } from '@/components/team/teamSelector';

import type { organization_rootQuery } from '@/queries/__generated__/organization_rootQuery.graphql';
import Box from '@mui/system/Box';
import { Dayjs } from 'dayjs';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<organization_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  defaultStartWeek: Dayjs;
};

const RootQuery = graphql`
  query organization_rootQuery(
    $organizationCustomDomain: String!
    $locationIds: [String!]!
    $teamIds: [String!]!
    $bookingsSearchCriteriaFrom: DateTime!
    $bookingsSearchCriteriaTo: DateTime!
    $locationsSortingValues: [LocationOrderInput!]
  ) {
    organization(customDomain: $organizationCustomDomain) {
      canModify
    }
    marketplaceBookingSubscriptionCancellationModes {
      type
      name
    }
    marketplaceBookingSubscriptions(first: 100, where: { includeMineOnly: true, organizationCustomDomain: $organizationCustomDomain }) {
      edges {
        node {
          id
          recurringBookings {
            id
          }
        }
      }
    }
    ...locationSelector_allLocations_query
    ...teamSelector_allTeams_query
    ...gettingStarted_query
    ...myBookings_query
    ...myBookings_bookings_query
  }
`;

const Organization = ({ queryReference, onReloadRequired, organizationCustomDomain, defaultStartWeek }: Props) => {
  const rootData = usePreloadedQuery<organization_rootQuery>(RootQuery, queryReference);
  const [today] = useState(startOfDay());
  const [startWeek, setStartWeek] = useState(defaultStartWeek);
  const [endWeek, setEndWeek] = useState(endOfWeek(defaultStartWeek).add(-1, 'milliseconds'));
  const [locationIds, setLocationIds] = useState<string[]>([]);
  const [teamIds, setTeamIds] = useState<string[]>([]);

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

  if (!organizationCustomDomain) {
    return null;
  }

  return (
    <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center' }}>
      <StackColumn sx={{ width: '100%', maxWidth: 1200, mx: 'auto', pt: { xs: 1, sm: 1, md: 2 } }} spacing={2}>
        <GettingStarted rootDataRelay={rootData} onReloadRequired={onReloadRequired} organizationCustomDomain={organizationCustomDomain} />

        <MyBookings
          rootDataRelay={rootData}
          rootDataBookingRelay={rootData}
          organizationCustomDomain={organizationCustomDomain}
          from={startWeek}
          to={endWeek}
          locationIds={locationIds}
          teamIds={teamIds}
          toolbar={
            <GridContainer spacing={1}>
              <LocationSelector rootDataRelay={rootData} onChange={handlLocationChanged} />
              <TeamSelector rootDataRelay={rootData} onChange={handlTeamChanged} />
              <WeekRangePicker defaultStartWeek={startWeek} onWeekChanged={handleWeehChanged} />
            </GridContainer>
          }
          hasTopInset={false}
          actions={<NewBookingButton onReloadRequired={onReloadRequired} defaultDate={today} organizationCustomDomain={organizationCustomDomain} />}
        />
      </StackColumn>
    </Box>
  );
};

const MemoOrganization = memo(Organization);

type RelayProps = {
  organizationCustomDomain: string;
};

const OrganizationWithRelay = ({ organizationCustomDomain }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organization_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);
  const [, startTransition] = useTransition();
  const [startWeek] = useState(startOfWeek());

  useEffect(() => {
    const bookingsSearchCriteriaFrom = startWeek.toISOString();
    const bookingsSearchCriteriaTo = endOfWeek(startWeek).add(-1, 'milliseconds').toISOString();

    loadQuery(
      {
        organizationCustomDomain,
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
  }, [loadQuery, triggerReload, startWeek, organizationCustomDomain]);

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
      <MemoOrganization queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationCustomDomain={organizationCustomDomain} defaultStartWeek={startWeek} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationWithRelay);
