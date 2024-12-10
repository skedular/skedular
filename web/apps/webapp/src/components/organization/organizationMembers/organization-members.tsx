import { TeamSelector } from '@/components/team/teamSelector';
import type { organizationMembers_organizationMembers_query$key } from '@/queries/__generated__/organizationMembers_organizationMembers_query.graphql';
import type { organizationMembers_organizationMembers_refetchableFragment } from '@/queries/__generated__/organizationMembers_organizationMembers_refetchableFragment.graphql';
import type { organizationMembers_rootQuery } from '@/queries/__generated__/organizationMembers_rootQuery.graphql';
import Box from '@mui/material/Box';
import Divider from '@mui/material/Divider';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import type { GridColDef } from '@mui/x-data-grid';
import { DataGrid, gridClasses } from '@mui/x-data-grid';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { Search } from '@repo/shared/components/search';
import { defaultPadding } from '@repo/shared/libs/theme';
import { getCustomerFullName } from '@repo/shared/libs/utils';
import { nanoid } from 'nanoid';
import { memo, useCallback, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<organizationMembers_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query organizationMembers_rootQuery($organizationId: String!, $peopleNameSearchText: String) {
    organization(id: $organizationId) {
      canInvitePeople
    }
    teams(where: { organizationId: $organizationId }) {
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
              }
            }
          }
          ...myTeamCard_TeamDetails
        }
      }
    }
    ...teamSelector_allTeams_query
    ...organizationMembers_organizationMembers_query
  }
`;

type CustomerDetails = {
  uniqueId: string;
  givenName?: string | null | undefined;
  middleName?: string | null | undefined;
  familyName?: string | null | undefined;
  name?: string | null | undefined;
  photoUrl?: string | null | undefined;
  phoneNumber?: string | null | undefined;
};
type RowType = {
  id: string;
  avatar: CustomerDetails;
  name: string;
  teams: string;
  email: string | null | undefined;
  phoneNumber: string | null | undefined;
  status: boolean;
};

const OrganizationMembers = ({ queryReference, onReloadRequired, organizationId }: Props) => {
  const rootData = usePreloadedQuery<organizationMembers_rootQuery>(RootQuery, queryReference);
  const [rootDataOrganizationMembers, refetchOrganizationMembers] = useRefetchableFragment<
    organizationMembers_organizationMembers_refetchableFragment,
    organizationMembers_organizationMembers_query$key
  >(
    graphql`
      fragment organizationMembers_organizationMembers_query on Query
      @refetchable(queryName: "organizationMembers_organizationMembers_refetchableFragment") {
        organizationMembers(where: { organizationId: $organizationId, nameContains: $peopleNameSearchText }) {
          __id
          totalCount
          edges {
            node {
              id
              customer {
                uniqueId
                email
                name
                givenName
                middleName
                familyName
                photoUrl
                phoneNumber
              }
              isActive
            }
          }
        }
      }
    `,
    rootData,
  );

  const [, startTransition] = useTransition();
  const [teamIds, setTeamIds] = useState<string[]>([]);
  const [peopleNameSearchText, setPeopleNameSearchText] = useState<string>('');
  const members = useMemo(() => {
    if (!rootDataOrganizationMembers.organizationMembers) {
      return [];
    }

    const members = rootDataOrganizationMembers.organizationMembers.edges
      .map(({ node }) => node)
      .sort((a, b) => {
        const name1 = getCustomerFullName(a.customer);
        const name2 = getCustomerFullName(b.customer);

        return name1.localeCompare(name2);
      })
      .map((member) => {
        const teams = rootData.teams
          ? rootData.teams.edges
              .map(({ node }) => node)
              .filter((item) => item.members.some(({ organizationMember }) => organizationMember?.customer.uniqueId === member.customer.uniqueId))
          : [];

        return {
          ...member,
          teams,
        };
      });

    return members.filter((member) => {
      if (teamIds.length === 0) {
        return true;
      }

      return member.teams.some((team) => teamIds.includes(team.id));
    });
  }, [rootData.teams, rootDataOrganizationMembers.organizationMembers, teamIds]);

  const handleRefetchOrganizationMembers = useCallback(
    (peopleNameSearchText: string) => {
      startTransition(() => {
        refetchOrganizationMembers(
          {
            peopleNameSearchText,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetchOrganizationMembers],
  );

  const handlTeamChanged = (id?: string) => {
    setTeamIds(id ? [id] : []);
  };

  const handleSearchTextChange = (str: string) => {
    setPeopleNameSearchText(str);

    handleRefetchOrganizationMembers(str);
  };

  const rows: RowType[] = members.map((member) => ({
    id: member.id,
    avatar: member.customer,
    name: getCustomerFullName(member.customer),
    teams: member.teams.map((team) => team.name).join(', '),
    email: member.customer.email,
    phoneNumber: member.customer.phoneNumber,
    status: member.isActive,
  }));

  const columns: GridColDef<(typeof rows)[number]>[] = [
    {
      field: 'avatar',
      headerName: '',
      editable: false,
      renderCell: (params) => (
        <CustomerAvatar key={params.value?.uniqueId} name={params.value} photo={{ url: params.value?.photoUrl }} size="medium" showFullName />
      ),
      display: 'flex',
    },
    {
      field: 'name',
      headerName: 'Name',
      editable: false,
      renderCell: (params) => params.value,
      display: 'text',
      minWidth: 200,
    },
    {
      field: 'teams',
      headerName: 'Team',
      editable: false,
      renderCell: (params) => params.value,
      display: 'text',
      minWidth: 350,
    },
    {
      field: 'email',
      headerName: 'Email',
      editable: false,
      renderCell: (params) => params.value,
      display: 'text',
      minWidth: 300,
    },
    {
      field: 'phoneNumber',
      headerName: 'Phone',
      editable: false,
      renderCell: (params) => params.value,
      display: 'text',
      minWidth: 300,
    },
    {
      field: 'status',
      headerName: 'Status',
      editable: false,
      renderCell: (params) => {
        return (
          <Stack
            direction="row"
            spacing={1}
            sx={{
              alignItems: 'center',
              flexWrap: 'wrap',
            }}
          >
            {params.value && (
              <>
                <Typography variant="body1">Active</Typography>
                <Box
                  sx={{
                    width: 15,
                    height: 15,
                    borderRadius: '50%',
                    backgroundColor: 'green',
                  }}
                />
              </>
            )}
            {!params.value && (
              <>
                <Typography variant="body1">Deactive</Typography>
                <Box
                  sx={{
                    width: 15,
                    height: 15,
                    borderRadius: '50%',
                    backgroundColor: 'orange',
                  }}
                />
              </>
            )}
          </Stack>
        );

        return params.value;
      },
      display: 'flex',
    },
  ];

  const rowCount = rows.length;

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
        <Search size="small" placeholder="Search for members" defaultValue={peopleNameSearchText} onChange={handleSearchTextChange} />
      </Stack>
      <Stack
        direction="column"
        spacing={1}
        sx={{
          paddingLeft: defaultPadding,
          paddingRight: defaultPadding,
        }}
      >
        <DataGrid
          rows={rows}
          columns={columns}
          hideFooterPagination={rowCount <= 10}
          initialState={{
            pagination: {
              rowCount,
              paginationModel: {
                pageSize: 10,
              },
            },
          }}
          pageSizeOptions={[10]}
          ignoreDiacritics
          disableRowSelectionOnClick
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
