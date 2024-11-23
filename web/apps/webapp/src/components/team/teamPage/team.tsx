import { Bookings } from '@/components/booking/bookingsPage';
import { TeamLink } from '@/components/team';
import type { team_rootQuery } from '@/queries/__generated__/team_rootQuery.graphql';
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
import TeamMembersTab from './team-members-tab';

type Props = {
  queryReference: PreloadedQuery<team_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId?: string;
  teamId: string;
};

const RootQuery = graphql`
  query team_rootQuery($organizationId: String!, $organizationExists: Boolean!, $teamId: String!) {
    organization(id: $organizationId) @include(if: $organizationExists) {
      id
      name
    }
    team(id: $teamId) {
      id
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

  let tabCount = 0;
  const bookingTabIndex = tabCount++;
  const aboutTabIndex = tabCount++;
  const membersTabIndex = tabCount++;

  if (tab === 'bookings') {
    initialTabIndex = bookingTabIndex;
  } else if (tab === 'about') {
    initialTabIndex = aboutTabIndex;
  } else if (tab === 'members') {
    initialTabIndex = membersTabIndex;
  }

  const [tabIndex, setTabIndex] = useState(initialTabIndex);

  const handleTabChange = (_: React.SyntheticEvent, newValue: number) => {
    setTabIndex(newValue);

    let tab = '';

    if (newValue === bookingTabIndex) {
      tab = 'bookings';
    } else if (newValue === aboutTabIndex) {
      tab = 'about';
    } else if (newValue === membersTabIndex) {
      tab = 'members';
    }

    if (tab) {
      router.push(`${getCurrentCompleteUrl()}?tab=${tab}`);
    }
  };

  if (!rootData.team) {
    return null;
  }

  return (
    <>
      <TeamLink organizationId={rootData.team.organization?.uniqueId} id={teamId} name={rootData.team?.name} excludeLink />

      <Tabs value={tabIndex} onChange={handleTabChange}>
        <Tab label="Bookings" />
        <Tab label="About" />
        <Tab label="Members" />
      </Tabs>

      {tabIndex === bookingTabIndex && <Bookings onReloadRequired={onReloadRequired} organizationId={organizationId} teamId={teamId} />}
      {tabIndex === aboutTabIndex && <TeamAboutTab onReloadRequired={onReloadRequired} organizationId={organizationId} teamId={teamId} />}
      {tabIndex === membersTabIndex && <TeamMembersTab onReloadRequired={onReloadRequired} organizationId={organizationId} teamId={teamId} />}
    </>
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
        organizationId: organizationId ?? '',
        organizationExists: !!organizationId,
        teamId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationId, teamId]);

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
