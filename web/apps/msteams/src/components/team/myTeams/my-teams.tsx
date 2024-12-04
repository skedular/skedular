import AvatarGroup from '@mui/material/AvatarGroup';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid2';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import type { GridColDef } from '@mui/x-data-grid';
import { DataGrid, gridClasses } from '@mui/x-data-grid';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { defaultPadding, defaultSpacing } from '@repo/shared/libs/theme';
import graphql from 'babel-plugin-relay/macro';
import { memo, startTransition, useCallback, useEffect, useMemo } from 'react';
import { useRefetchableFragment } from 'react-relay';
import type { myTeams_teams_query$key } from './__generated__/myTeams_teams_query.graphql';
import type { myTeams_teams_refetchableFragment } from './__generated__/myTeams_teams_refetchableFragment.graphql';
import MyTeamCard from './my-team-card';

type Props = {
  rootDataRelay: myTeams_teams_query$key;
  onReloadRequired: () => void;
  primaryLocationIds: string[];
  viewMode: 'list' | 'grid';
};

type TeamDetails = {
  name: string;
};

type CustomerDetails = {
  uniqueId: string;
  givenName?: string | null | undefined;
  middleName?: string | null | undefined;
  familyName?: string | null | undefined;
  name?: string | null | undefined;
  photoUrl?: string | null | undefined;
};

type RowType = {
  id: string;
  team: TeamDetails;
  teammates: ReadonlyArray<CustomerDetails>;
};

const MyTeams = ({ rootDataRelay, onReloadRequired, primaryLocationIds, viewMode }: Props) => {
  const [rootDataRefetchable, refetch] = useRefetchableFragment<
    myTeams_teams_refetchableFragment,
    myTeams_teams_query$key
  >(
    graphql`
      fragment myTeams_teams_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "myTeams_teams_refetchableFragment") {
        teams(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, primaryLocationIds: $primaryLocationIds }
          orderBy: $teamsSortingValues
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
    rootDataRelay,
  );

  const connectionIds = useMemo(() => (rootDataRefetchable.teams ? [rootDataRefetchable.teams.__id] : []), [rootDataRefetchable.teams]);
  const teams = useMemo(() => {
    if (!rootDataRefetchable.teams) {
      return [];
    }

    return rootDataRefetchable.teams.edges.map((edge) => edge.node).sort((a, b) => a.name.localeCompare(b.name));
  }, [rootDataRefetchable.teams]);

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

  useEffect(() => handleRefetch(primaryLocationIds), [handleRefetch, primaryLocationIds]);

  const rows: RowType[] = teams.map((team) => {
    return {
      id: team.id,
      team,
      teammates: team.members.filter(({ organizationMember }) => !!organizationMember).map(({ organizationMember }) => organizationMember!.customer),
    };
  });

  const columns: GridColDef<(typeof rows)[number]>[] = [
    {
      field: 'team',
      headerName: 'Team',
      editable: false,
      renderCell: (params) => params.value.name,
      display: 'text',
      minWidth: 200,
    },
    {
      field: 'teammates',
      headerName: 'Members of this team',
      editable: false,
      renderCell: (params) => (
        <AvatarGroup max={5}>
          {params.value.map((customer: CustomerDetails) => (
            <CustomerAvatar key={customer?.uniqueId} name={customer} photo={{ url: customer?.photoUrl }} size="medium" showFullName />
          ))}
        </AvatarGroup>
      ),
      display: 'flex',
      minWidth: 300,
    },
  ];

  if (!rootDataRefetchable.teams) {
    return <></>;
  }

  return (
    <Stack
      direction="column"
      spacing={1}
      sx={{
        paddingLeft: defaultPadding,
        paddingRight: defaultPadding,
        paddingTop: defaultPadding,
      }}
    >
      <Typography variant="h5">My Teams</Typography>

      <Divider />

      {viewMode === 'grid' && (
        <Grid container spacing={defaultSpacing} sx={{ alignItems: 'flex-start' }}>
          {teams.map((team) => (
            <Grid key={team.id}>
              <MyTeamCard
                teamDetailsRelay={team}
                connectionIds={connectionIds}
                teammates={team.members
                  .filter(({ organizationMember }) => !!organizationMember)!
                  .map(({ organizationMember }) => organizationMember!.customer)}
              />
            </Grid>
          ))}
        </Grid>
      )}

      {viewMode === 'list' && (
        <DataGrid
          rows={rows}
          columns={columns}
          ignoreDiacritics
          disableRowSelectionOnClick
          hideFooter
          getRowHeight={() => 'auto'}
          rowSpacingType="margin"
          getRowSpacing={() => ({ top: 3, bottom: 3 })}
          sx={{
            [`& .${gridClasses.cell}`]: {
              paddingTop: 1,
              paddingBottom: 1,
            },
            [`& .${gridClasses.row}`]: {
              paddingLeft: 1,
              paddingTop: 1,
              paddingBottom: 1,
              borderRadius: 2,
              backgroundColor: (theme) => theme.palette.background.paper,
            },
          }}
        />
      )}
    </Stack>
  );
};

export default memo(MyTeams);
