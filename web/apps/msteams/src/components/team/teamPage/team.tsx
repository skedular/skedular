import Stack from '@mui/material/Stack';
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import graphql from 'babel-plugin-relay/macro';
import { TeamLink } from 'components/team';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { useSearchParams } from 'react-router-dom';
import type { team_rootQuery } from './__generated__/team_rootQuery.graphql';
import TeamAboutTab from './team-about-tab';
import TeamBookingsTab from './team-bookings-tab';
import TeamPeopleTab from './team-people-tab';

type Props = {
  queryReference: PreloadedQuery<team_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  teamId: string;
};

const RootQuery = graphql`
  query team_rootQuery(
    $organizationId: String!
    $organizationExists: Boolean!
    $teamId: String!
    $teamExists: Boolean!
    $bookingPeopleNameSearchText: String
    $teamPeopleSortingValues: [TeamMemberOrderInput!]
    $organizationMemberSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]
    $peopleNameSearchText: String
  ) {
    team(id: $teamId) {
      name
      organization {
        uniqueId
      }
    }
    ...teamPeopleTab_query
  }
`;

const Team = ({ queryReference, onReloadRequired, organizationId, teamId }: Props) => {
  const rootData = usePreloadedQuery<team_rootQuery>(RootQuery, queryReference);
  const [searchParams, setSearchParams] = useSearchParams();
  const tab = searchParams.get('tab');
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
      setSearchParams({ tab });
    }
  };

  if (!rootData.team) {
    return null;
  }

  return (
    <Stack direction="column" spacing={1}>
      <TeamLink organizationId={organizationId} id={teamId} name={rootData.team?.name} excludeLink />

      <Tabs value={tabIndex} onChange={handleTabChange}>
        <Tab label="Bookings" />
        <Tab label="About" />
        <Tab label="People" />
      </Tabs>

      <>
        {tabIndex === 0 && <TeamBookingsTab onReloadRequired={onReloadRequired} organizationId={organizationId} teamId={teamId} />}
        {tabIndex === 1 && <TeamAboutTab onReloadRequired={onReloadRequired} organizationId={organizationId} teamId={teamId} />}
        {tabIndex === 2 && <TeamPeopleTab rootDataRelay={rootData} organizationId={organizationId} teamId={teamId} />}
      </>
    </Stack>
  );
};

const MemoTeam = memo(Team);

type RelayProps = {
  organizationId: string;
  teamId: string;
};

const TeamWithRelay = ({ organizationId, teamId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<team_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        teamId,
        teamExists: !!teamId,
        organizationId,
        organizationExists: !!organizationId,
        teamPeopleSortingValues: [
          {
            direction: 'Ascending',
            field: 'name',
          },
        ],
        organizationMemberSelectorOrganizationMembersSortingValues: [
          {
            direction: 'Ascending',
            field: 'name',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReload, organizationId, teamId]);

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
      <MemoTeam queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} teamId={teamId} />
    </ErrorBoundary>
  );
};

export default memo(TeamWithRelay);
