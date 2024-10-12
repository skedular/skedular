import { TeamLink } from '@/components/team';
import type { team_rootQuery } from '@/queries/__generated__/team_rootQuery.graphql';
import Stack from '@mui/material/Stack';
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { getCurrentCompleteUrl } from '@repo/shared/libs/utils';
import { nanoid } from 'nanoid';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';
import TeamAboutTab from './team-about-tab';
import TeamBookingsTab from './team-bookings-tab';
import TeamPeopleTab from './team-people-tab';

type Props = {
  queryReference: PreloadedQuery<team_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId?: string;
  teamId: string;
};

const RootQuery = graphql`
  query team_rootQuery($teamId: String!) {
    team(id: $teamId) {
      name
      organization {
        uniqueId
      }
    }
  }
`;

const Team = ({ queryReference, onReloadRequired, organizationId, teamId }: Props) => {
  const rootData = usePreloadedQuery<team_rootQuery>(RootQuery, queryReference);
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
        {tabIndex === 0 && <TeamBookingsTab onReloadRequired={onReloadRequired} organizationId={organizationId} teamId={teamId} />}
        {tabIndex === 1 && <TeamAboutTab onReloadRequired={onReloadRequired} organizationId={organizationId} teamId={teamId} />}
        {tabIndex === 2 && <TeamPeopleTab onReloadRequired={onReloadRequired} organizationId={organizationId} teamId={teamId} />}
      </>
    </Stack>
  );
};

const MemoTeam = memo(Team);

type RelayProps = {
  organizationId?: string;
  teamId: string;
};

const TeamWithRelay = ({ organizationId, teamId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<team_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        teamId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, teamId]);

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
      <MemoTeam queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} teamId={teamId} />
    </ErrorBoundary>
  );
};

export default memo(TeamWithRelay);
