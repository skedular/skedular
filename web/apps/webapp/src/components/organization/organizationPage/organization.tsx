import { NewBookingButton } from '@/components/booking/addBooking';
import { MyBookings } from '@/components/booking/myBookings';
import { GridContainer, PushToRight, StackColumn } from '@/components/commons';
import { WeekRangePicker } from '@/components/datePickers';
import { GettingStarted } from '@/components/gettingStarted';
import { getOrganizationLocationsBaseLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { ClaimLocationOwnershipButton } from '@/components/location';
import { LocationSelector } from '@/components/location/locationSelector';
import { RelayError, toRootError } from '@/components/relayError';
import { TeamSelector } from '@/components/team/teamSelector';
import { useIntegratedPlatrform } from '@/libs/providers';
import { defaultPadding, maxScreenWidth } from '@/libs/theme';
import { endOfWeek, startOfDay, startOfWeek } from '@/libs/utils';
import type { organization_rootQuery } from '@/queries/__generated__/organization_rootQuery.graphql';
import type { SxProps, Theme } from '@mui/system';
import Box from '@mui/system/Box';
import { Dayjs } from 'dayjs';
import { useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

const filterSurfaceSx: SxProps<Theme> = {
  borderRadius: 4,
  border: 1,
  borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
  backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255, 255, 255, 0.88)' : theme.palette.background.paper),
  boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 8px 24px rgba(15, 23, 42, 0.06)' : '0 1px 3px rgba(0, 0, 0, 0.24)'),
};

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
    ...locationSelector_allLocations_query
    ...teamSelector_allTeams_query
    ...gettingStarted_query
    ...myBookings_query
    ...myBookings_bookings_query
  }
`;

const Organization = ({ queryReference, onReloadRequired, organizationCustomDomain, defaultStartWeek }: Props) => {
  const rootData = usePreloadedQuery<organization_rootQuery>(RootQuery, queryReference);
  const { integratedPlatrform } = useIntegratedPlatrform();
  const [today] = useState(startOfDay());
  const [startWeek, setStartWeek] = useState(defaultStartWeek);
  const [endWeek, setEndWeek] = useState(endOfWeek(defaultStartWeek).add(-1, 'milliseconds'));
  const [locationIds, setLocationIds] = useState<string[]>([]);
  const [teamIds, setTeamIds] = useState<string[]>([]);
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

  const handleClaimLocationOwnershipClicked = () => {
    router.push(getOrganizationLocationsBaseLink(integratedPlatrform, organizationCustomDomain));
  };

  if (!organizationCustomDomain) {
    return null;
  }

  return (
    <StackColumn sx={{ maxWidth: maxScreenWidth, width: '100%' }} spacing={2}>
      <Box sx={{ paddingTop: defaultPadding, paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
        <Box sx={{ ...filterSurfaceSx, px: 2, py: 1.5 }}>
          <GridContainer spacing={1}>
            <LocationSelector rootDataRelay={rootData} onChange={handlLocationChanged} />
            <TeamSelector rootDataRelay={rootData} onChange={handlTeamChanged} />
            <WeekRangePicker defaultStartWeek={startWeek} onWeekChanged={handleWeehChanged} />
            <PushToRight />
            <NewBookingButton onReloadRequired={onReloadRequired} defaultDate={today} organizationCustomDomain={organizationCustomDomain} />
            {rootData.organization?.canModify && (
              <ClaimLocationOwnershipButton organizationCustomDomain={organizationCustomDomain} onClaimClicked={handleClaimLocationOwnershipClicked} />
            )}
          </GridContainer>
        </Box>
      </Box>
      <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
        <GettingStarted rootDataRelay={rootData} onReloadRequired={onReloadRequired} organizationCustomDomain={organizationCustomDomain} />
      </Box>
      <MyBookings
        rootDataRelay={rootData}
        rootDataBookingRelay={rootData}
        organizationCustomDomain={organizationCustomDomain}
        from={startWeek}
        to={endWeek}
        locationIds={locationIds}
        teamIds={teamIds}
      />
    </StackColumn>
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
