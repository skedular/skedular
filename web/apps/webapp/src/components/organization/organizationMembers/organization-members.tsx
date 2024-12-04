import { TeamSelector } from '@/components/team/teamSelector';
import type { organizationMembers_rootQuery } from '@/queries/__generated__/organizationMembers_rootQuery.graphql';
import type { organizationMembers_teams_query$key } from '@/queries/__generated__/organizationMembers_teams_query.graphql';
import type { organizationMembers_teamsrefetchableFragment } from '@/queries/__generated__/organizationMembers_teamsrefetchableFragment.graphql';
import Box from '@mui/material/Box';
import Divider from '@mui/material/Divider';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { defaultPadding } from '@repo/shared/libs/theme';
import { nanoid } from 'nanoid';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<organizationMembers_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query organizationMembers_rootQuery($organizationId: String!) {
    organization(id: $organizationId) {
      members {
        id
        customer {
          uniqueId
          name
          givenName
          middleName
          familyName
          photoUrl
        }
      }
    }
    ...teamSelector_allTeams_query
    ...organizationMembers_teams_query
  }
`;

const OrganizationMembers = ({ queryReference, onReloadRequired, organizationId }: Props) => {
  const rootData = usePreloadedQuery<organizationMembers_rootQuery>(RootQuery, queryReference);
  const [rootDataRefetchable, refetch] = useRefetchableFragment<
    organizationMembers_teamsrefetchableFragment,
    organizationMembers_teams_query$key
  >(
    graphql`
      fragment organizationMembers_teams_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationMembers_teamsrefetchableFragment") {
        teams(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId }
        ) @connection(key: "myTeams_teams") {
          __id
          totalCount
          edges {
            node {
              id
              name
              members {
                organizationMember {
                  uniqueId
                  customer {
                    uniqueId
                    givenName
                    middleName
                    familyName
                    name
                    photoUrl
                  }
                }
              }
              ...myTeamCard_TeamDetails
            }
          }
        }
      }
    `,
    rootData,
  );

  const [teamIds, setTeamIds] = useState<string[]>([]);

  const handlTeamChanged = (id?: string) => {
    setTeamIds(id ? [id] : []);
  };

  return (
    <Stack direction="column" spacing={1}>
      <Stack
        direction="column"
        spacing={1}
        sx={{
          paddingLeft: defaultPadding,
          paddingRight: defaultPadding,
          paddingTop: defaultPadding,
        }}
      >
        <Typography variant="h5">Organization Members</Typography>
        <Typography variant="body1">View members in your organization</Typography>
        <Divider />
      </Stack>
      <Stack
        direction="row"
        spacing={1}
        sx={{
          alignItems: 'center',
          flexWrap: 'wrap',
          paddingLeft: defaultPadding,
          paddingRight: defaultPadding,
          paddingBottom: defaultPadding,
          paddingTop: defaultPadding,
        }}
      >
        <TeamSelector rootDataRelay={rootData} onChange={handlTeamChanged} />
        <Box sx={{ flexGrow: 1 }} /> {/* This will push NewBookingButton to the right */}
      </Stack>
    </Stack>
  );
};

const MemoOrganizationMembers = memo(OrganizationMembers);

type RelayProps = {
  organizationId: string;
};

const OrganizationMembersWithRelay = ({ organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationMembers_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationId]);

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
      <MemoOrganizationMembers queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationMembersWithRelay);
