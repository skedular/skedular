import type { organizationTeam_rootQuery } from '@/queries/__generated__/organizationTeam_rootQuery.graphql';
import Box from '@mui/material/Box';
import Divider from '@mui/material/Divider';
import { BodyIconTypography, SectionIconTypography, StackColumn } from '@repo/shared/components/commons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { defaultPadding, maxScreenWidth } from '@repo/shared/libs/theme';
import { nanoid } from 'nanoid';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import OrganizationTeamLeftSideNavigationMenuContent from './organization-team-left-side-navigation-menu-content';

type Props = {
  queryReference: PreloadedQuery<organizationTeam_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  teamId: string;
};

const RootQuery = graphql`
  query organizationTeam_rootQuery($teamId: String!) {
    team(id: $teamId) {
      id
      name
    }
  }
`;

const OrganizationTeam = ({ queryReference, organizationId, teamId }: Props) => {
  const rootData = usePreloadedQuery<organizationTeam_rootQuery>(RootQuery, queryReference);

  return (
    <Box sx={{ display: 'flex', width: '100%' }}>
      <OrganizationTeamLeftSideNavigationMenuContent organizationId={organizationId} teamId={teamId} hideIcons />
      <Box>
        <StackColumn sx={{ maxWidth: maxScreenWidth }}>
          <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
            <SectionIconTypography label="Team Setup" />
            <BodyIconTypography label="Edit your team name and details" />
            <Divider />
          </StackColumn>

          <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
            <SectionIconTypography label="Team Members" />
            <BodyIconTypography label="Manage your team members" />
            <Divider />
          </StackColumn>
        </StackColumn>
      </Box>
    </Box>
  );
};

const MemoOrganizationTeam = memo(OrganizationTeam);

type RelayProps = {
  organizationId: string;
  teamId: string;
};

const OrganizationTeamWithRelay = ({ organizationId, teamId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationTeam_rootQuery>(RootQuery);
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
      <MemoOrganizationTeam queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} teamId={teamId} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationTeamWithRelay);
