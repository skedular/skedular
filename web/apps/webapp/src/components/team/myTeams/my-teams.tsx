import type { myTeams_teams_availableOrganizationDesks_query$key } from '@/queries/__generated__/myTeams_teams_availableOrganizationDesks_query.graphql';
import type { myTeams_teams_availableOrganizationDesks_refetchableFragment } from '@/queries/__generated__/myTeams_teams_availableOrganizationDesks_refetchableFragment.graphql';
import AvatarGroup from '@mui/material/AvatarGroup';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid2';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import type { GridColDef } from '@mui/x-data-grid';
import { DataGrid, gridClasses } from '@mui/x-data-grid';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { LocationIcon } from '@repo/shared/components/icons';
import { defaultPadding, defaultSpacing } from '@repo/shared/libs/theme';
import { memo, startTransition, useCallback, useEffect, useMemo } from 'react';
import { graphql, useRefetchableFragment } from 'react-relay';

type Props = {
  rootDataRelay: myTeams_teams_availableOrganizationDesks_query$key;
  onReloadRequired: () => void;
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

const MyLocations = ({ rootDataRelay, onReloadRequired, viewMode }: Props) => {
  const [rootData, refetch] = useRefetchableFragment<
    myTeams_teams_availableOrganizationDesks_refetchableFragment,
    myTeams_teams_availableOrganizationDesks_query$key
  >(
    graphql`
      fragment myTeams_teams_availableOrganizationDesks_query on Query
      @refetchable(queryName: "myTeams_teams_availableOrganizationDesks_refetchableFragment") {
        teams(where: { organizationId: $organizationId }, orderBy: $teamsSortingValues) {
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
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const teams = useMemo(() => {
    if (!rootData.teams) {
      return [];
    }

    return rootData.teams.edges.map((edge) => edge.node).sort((a, b) => a.name.localeCompare(b.name));
  }, [rootData.teams]);

  const handleRefetchAllBookings = useCallback(() => {
    startTransition(() => {
      refetch(
        {},
        {
          fetchPolicy: 'store-and-network',
        },
      );
    });
  }, [refetch]);

  useEffect(() => handleRefetchAllBookings(), [handleRefetchAllBookings]);

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

  if (!rootData.teams) {
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
          {teams.map((team) => {
            return (
              <Grid key={team.id}>
                <Card sx={{ width: 600 }}>
                  <CardHeader
                    title={
                      <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                        <LocationIcon fontSize="medium" />
                        <Typography variant="h6">{team.name}</Typography>
                      </Stack>
                    }
                  />
                  <CardContent>
                    <Stack direction="column" spacing={1} sx={{ paddingTop: 1, paddingBottom: 1 }}>
                      <Typography variant="body1">Members of this team</Typography>
                      <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                        <AvatarGroup max={5}>
                          {team.members
                            .filter(({ organizationMember }) => !!organizationMember)
                            .map(({ organizationMember }) => (
                              <CustomerAvatar
                                key={organizationMember!.customer?.uniqueId}
                                name={organizationMember!.customer}
                                photo={{ url: organizationMember!.customer?.photoUrl }}
                                size="medium"
                                showFullName
                              />
                            ))}
                        </AvatarGroup>
                      </Stack>
                    </Stack>
                  </CardContent>
                </Card>
              </Grid>
            );
          })}
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

export default memo(MyLocations);
