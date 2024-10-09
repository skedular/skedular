import { TeamLink } from '@/components/team';
import type { teamPage_query$key } from '@/queries/__generated__/teamPage_query.graphql';
import Stack from '@mui/material/Stack';
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import { getCurrentCompleteUrl } from '@repo/shared/libs/utils';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useState } from 'react';
import { graphql, useFragment } from 'react-relay';
import TeamAboutTab from './team-about-tab';
import TeamBookingsTab from './team-bookings-tab';
import TeamPeopleTab from './team-people-tab';

type Props = {
  rootDataRelay: teamPage_query$key;
  teamId: string;
  organizationId: string;
};

const RootQuery = graphql`
  query location_rootQuery(
    $organizationId: String!
    $organizationExists: Boolean!
    $locationId: String!
    $locationExists: Boolean!
    $zoneTagType: String!
    $dateToGetAvailableDesks: DateTime!
    $deskIdsToIncludeToGetAvailableDesks: [String!]!
    $fromToGetBookings: DateTime
    $toToGetBookings: DateTime
    $peopleNameSearchText: String
    $zoneNameSearchText: String
    $deskNameSearchText: String
    $bookingPeopleNameSearchText: String
    $bookingSortingValues: [BookingOrderInput!]!
    $locationPeopleSortingValues: [LocationMemberOrderInput!]
    $locationOrganizationPeopleSortingValues: [CustomerOrderInput!]
    $zoneSortingValues: [LocationTagOrderInput!]!
    $deskSortingValues: [DeskOrderInput!]!
    $bookingDetailsSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]
    $deskMultipleChoicesZonesSortingValues: [LocationTagOrderInput!]
    $bookingsSearchCriteriaFrom: DateTime!
    $bookingsSearchCriteriaUntil: DateTime!
  ) {
    team(id: $teamId) {
      name
      organization {
        uniqueId
      }
    }
    ...teamBookingsTab_query
    ...teamAboutTab_query
    ...teamPeopleTab_query
  }
`;

const Team = ({ rootDataRelay, teamId, organizationId }: Props) => {
  const rootData = useFragment<teamPage_query$key>(
    graphql`
      fragment teamPage_query on Query {
        team(id: $teamId) {
          name
          organization {
            uniqueId
          }
        }
        ...teamBookingsTab_query
        ...teamAboutTab_query
        ...teamPeopleTab_query
      }
    `,
    rootDataRelay,
  );

  const searchParams = useSearchParams();
  const tab = searchParams.get('tab');
  const router = useRouter();
  let initialTabIndex = 0;

  if (tab === 'bookings') {
    initialTabIndex = 0;
  } else if (tab === 'about') {
    initialTabIndex = 1;
  } else if (tab === 'people') {
    initialTabIndex = 2;
  }

  const [tabIndex, setTabIndex] = useState(initialTabIndex);

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setTabIndex(newValue);

    let tab = '';

    if (newValue === 0) {
      tab = 'bookings';
    } else if (newValue === 1) {
      tab = 'about';
    } else if (newValue === 2) {
      tab = 'people';
    }

    if (tab) {
      router.push(`${getCurrentCompleteUrl()}?tab=${tab}`);
    }
  };

  if (!rootData.team) {
    return null;
  }

  return (
    <Stack direction="column" spacing={1}>
      <TeamLink organizationId={rootData.team.organization?.uniqueId} id={teamId} name={rootData.team?.name} excludeLink />

      <Tabs value={tabIndex} onChange={handleTabChange}>
        <Tab label="Bookings" />
        <Tab label="About" />
        <Tab label="People" />
      </Tabs>

      <>
        {tabIndex === 0 && <TeamBookingsTab rootDataRelay={rootData} organizationId={organizationId} teamId={teamId} />}
        {tabIndex === 1 && <TeamAboutTab rootDataRelay={rootData} organizationId={organizationId} />}
        {tabIndex === 2 && <TeamPeopleTab rootDataRelay={rootData} organizationId={organizationId} teamId={teamId} />}
      </>
    </Stack>
  );
};

export default memo(Team);
