import { Bookings } from '@/components/booking/bookingsPage';
import { getOrganizationBaseLink } from '@/components/organization';
import { TeamLink, getTeamBaseLink } from '@/components/team';
import type { team_rootQuery } from '@/queries/__generated__/team_rootQuery.graphql';
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { UpdateBreadcrumpsContext } from '@repo/shared/libs/providers';
import { getCurrentCompleteUrl } from '@repo/shared/libs/utils';
import { nanoid } from 'nanoid';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
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
  const updateBreadcrumps = useContext(UpdateBreadcrumpsContext);
  let initialTabIndex = 0;

  useEffect(() => {
    let breadcrumbs = new Map<string, string>();

    if (rootData.organization) {
      breadcrumbs = breadcrumbs.set(getOrganizationBaseLink(rootData.organization.id), rootData.organization?.name!);
    }

    if (rootData.team) {
      breadcrumbs = breadcrumbs.set(getTeamBaseLink(rootData.team.id, rootData.organization?.id), rootData.team?.name!);
    }

    updateBreadcrumps(breadcrumbs);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [rootData.organization, rootData.team]);

  if (tab === 'bookings') {
    initialTabIndex = 0;
  } else if (tab === 'about') {
    initialTabIndex = 1;
  } else if (tab === 'people') {
    initialTabIndex = 2;
  }

  const [tabIndex, setTabIndex] = useState(initialTabIndex);

  const handleTabChange = (_: React.SyntheticEvent, newValue: number) => {
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
    <>
      <TeamLink organizationId={rootData.team.organization?.uniqueId} id={teamId} name={rootData.team?.name} excludeLink />

      <Tabs value={tabIndex} onChange={handleTabChange}>
        <Tab label="Bookings" />
        <Tab label="About" />
        <Tab label="People" />
      </Tabs>

      {tabIndex === 0 && <Bookings onReloadRequired={onReloadRequired} organizationId={organizationId} teamId={teamId} />}
      {tabIndex === 1 && <TeamAboutTab onReloadRequired={onReloadRequired} organizationId={organizationId} teamId={teamId} />}
      {tabIndex === 2 && <TeamMembersTab onReloadRequired={onReloadRequired} organizationId={organizationId} teamId={teamId} />}
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
