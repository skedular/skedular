import { Loading } from '@/components/loading';
import { LocationSelector } from '@/components/location/locationSelector';
import { RelayError, toRootError } from '@skedular/shared';
import { NewTeamButton } from '@/components/team/addTeam';
import type { organizationTeams_rootQuery } from '@/queries/__generated__/organizationTeams_rootQuery.graphql';
import type { organizationTeams_teams_query$key } from '@/queries/__generated__/organizationTeams_teams_query.graphql';
import type { organizationTeams_teams_refetchableFragment } from '@/queries/__generated__/organizationTeams_teams_refetchableFragment.graphql';
import Box from '@mui/system/Box';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, startTransition, useCallback, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';
import { v7 as uuid } from 'uuid';
import OrganizationTeamsPageShell from './organization-teams-page-shell';
import TeamCard from './team-card';

type Props = {
  queryReference: PreloadedQuery<organizationTeams_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
};

const RootQuery = graphql`
  query organizationTeams_rootQuery(
    $organizationCustomDomain: String!
    $primaryLocationIds: [String!]
    $teamsSortingValues: [TeamOrderInput!]
    $locationsSortingValues: [LocationOrderInput!]
  ) {
    me {
      id
    }
    ...locationSelector_allLocations_query
    ...organizationTeams_teams_query
  }
`;

const Teams = ({ queryReference, organizationCustomDomain }: Props) => {
  const rootData = usePreloadedQuery<organizationTeams_rootQuery>(RootQuery, queryReference);
  const router = useRouter();
  const searchParams = useSearchParams();
  const locationId = searchParams.get('locationId');
  const [rootDataRefetchable, refetch] = useRefetchableFragment<organizationTeams_teams_refetchableFragment, organizationTeams_teams_query$key>(
    graphql`
      fragment organizationTeams_teams_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationTeams_teams_refetchableFragment") {
        teams(first: $count, after: $cursor, where: { organizationCustomDomain: $organizationCustomDomain, primaryLocationIds: $primaryLocationIds }, orderBy: $teamsSortingValues)
          @connection(key: "organizationTeams_teams") {
          __id
          totalCount
          edges {
            node {
              id
              name
              organization {
                customDomain
              }
              members {
                edges {
                  node {
                    organizationMember {
                      uniqueId
                      customer {
                        id
                        givenName
                        middleName
                        familyName
                        name
                        photoUrl
                      }
                    }
                  }
                }
              }
              canModify
              canDelete
              ...teamCard_TeamDetails
            }
          }
        }
      }
    `,
    rootData,
  );

  const locationIds = useMemo(() => (locationId ? [locationId] : []), [locationId]);
  const connectionIds = useMemo(() => [rootDataRefetchable.teams.__id], [rootDataRefetchable.teams]);
  const teams = useMemo(() => rootDataRefetchable.teams.edges.map((edge) => edge.node), [rootDataRefetchable.teams]);

  const handleRefetch = useCallback(
    (primaryLocationIds: string[]) => {
      startTransition(() => {
        refetch(
          {
            primaryLocationIds,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetch],
  );

  useEffect(() => handleRefetch(locationIds), [handleRefetch, locationIds]);

  const handleLocationChanged = (id?: string) => {
    const params = new URLSearchParams(window.location.search);
    if (id) params.set('locationId', id);
    else params.delete('locationId');
    router.push(`?${params.toString()}`);
  };

  if (!rootDataRefetchable.teams) {
    return null;
  }

  const pageActions = <NewTeamButton organizationCustomDomain={organizationCustomDomain} />;
  const pageToolbar = <LocationSelector key={`location-${locationId ?? 'all'}`} rootDataRelay={rootData} onChange={handleLocationChanged} defaultValue={locationId} />;

  return (
    <OrganizationTeamsPageShell actions={pageActions} toolbar={pageToolbar} isEmpty={teams.length === 0}>
      <Box
        sx={{
          display: 'grid',
          gridTemplateColumns: {
            xs: '1fr',
            sm: 'repeat(auto-fit, minmax(320px, 360px))',
          },
          gap: 2,
          alignItems: 'stretch',
          justifyContent: 'start',
        }}
      >
        {teams.map((team) => (
          <TeamCard
            key={team.id}
            teamDetailsRelay={team}
            connectionIds={connectionIds}
            teammates={team.members.edges
              .map(({ node }) => node)
              .filter(({ organizationMember }) => !!organizationMember)!
              .map(({ organizationMember }) => organizationMember!.customer)}
          />
        ))}
      </Box>
    </OrganizationTeamsPageShell>
  );
};

const MemoTeams = memo(Teams);

type RelayProps = {
  organizationCustomDomain: string;
};

const TeamsWithRelay = ({ organizationCustomDomain }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationTeams_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const searchParams = useSearchParams();
  const locationId = searchParams.get('locationId');

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
        primaryLocationIds: locationId ? [locationId] : [],
        teamsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
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
  }, [loadQuery, triggerReloadId, organizationCustomDomain, locationId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoTeams queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationCustomDomain={organizationCustomDomain} />
    </ErrorBoundary>
  );
};

export default memo(TeamsWithRelay);
