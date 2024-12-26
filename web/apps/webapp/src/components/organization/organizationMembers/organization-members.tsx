import { TeamSelector } from '@/components/team/teamSelector';
import type { organizationMembers_changeOrganizationMembersStatusMutation } from '@/queries/__generated__/organizationMembers_changeOrganizationMembersStatusMutation.graphql';
import type { organizationMembers_organizationMembers_query$key } from '@/queries/__generated__/organizationMembers_organizationMembers_query.graphql';
import type { organizationMembers_organizationMembers_refetchableFragment } from '@/queries/__generated__/organizationMembers_organizationMembers_refetchableFragment.graphql';
import type { organizationMembers_removeOrganizationMembersInputMutation } from '@/queries/__generated__/organizationMembers_removeOrganizationMembersInputMutation.graphql';
import type { organizationMembers_rootQuery } from '@/queries/__generated__/organizationMembers_rootQuery.graphql';
import { Button } from '@mui/material';
import Box from '@mui/material/Box';
import Divider from '@mui/material/Divider';
import type { GridColDef, GridRowSelectionModel } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { BodyIconTypography, PushToRight, SectionIconTypography, SmallIconTypography, StackColumn, StackRow } from '@repo/shared/components/commons';
import { DeleteIcon } from '@repo/shared/components/icons';
import { Loading } from '@repo/shared/components/loading';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { Search } from '@repo/shared/components/search';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { defaultGridStyle, defaultPadding, emerald, flame, maxScreenWidth } from '@repo/shared/libs/theme';
import { getCustomerFullName, joinErrors } from '@repo/shared/libs/utils';
import { nanoid } from 'nanoid';
import { memo, useCallback, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { expandedDrawerWidthPx } from './commons';
import OrganizationMembersLeftSideNavigationMenuContent from './organization-members-left-side-navigation-menu-content';

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

const OrganizationMembers = ({ queryReference, organizationId }: Props) => {
  const rootData = usePreloadedQuery<organizationMembers_rootQuery>(RootQuery, queryReference);
  const [rootDataOrganizationMembers, refetchOrganizationMembers] = useRefetchableFragment<
    organizationMembers_organizationMembers_refetchableFragment,
    organizationMembers_organizationMembers_query$key
  >(
    graphql`
      fragment organizationMembers_organizationMembers_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationMembers_organizationMembers_refetchableFragment") {
        organizationMembers(first: $count, after: $cursor, where: { organizationId: $organizationId, nameContains: $peopleNameSearchText })
          @connection(key: "organizationMembers_organizationMembers") {
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
              status
            }
          }
        }
      }
    `,
    rootData,
  );

  const [commitChangeOrganizationMembersStatus] = useMutation<organizationMembers_changeOrganizationMembersStatusMutation>(graphql`
    mutation organizationMembers_changeOrganizationMembersStatusMutation($input: ChangeOrganizationMembersStatusInput!) {
      changeOrganizationMembersStatus(input: $input) {
        members {
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
          status
        }
      }
    }
  `);

  const [commitRemoveOrganizationMembersInput] = useMutation<organizationMembers_removeOrganizationMembersInputMutation>(graphql`
    mutation organizationMembers_removeOrganizationMembersInputMutation($connectionIds: [ID!]!, $input: RemoveOrganizationMembersInput!) {
      removeOrganizationMembersInput(input: $input) {
        members {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [, startTransition] = useTransition();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [teamIds, setTeamIds] = useState<string[]>([]);
  const [peopleNameSearchText, setPeopleNameSearchText] = useState<string>('');
  const [seledctedMembers, setSeledctedMembers] = useState<GridRowSelectionModel>([]);

  const connectionIds = useMemo(
    () => (rootDataOrganizationMembers.organizationMembers ? [rootDataOrganizationMembers.organizationMembers.__id] : []),
    [rootDataOrganizationMembers.organizationMembers],
  );
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

  const handleSelectedMembersChanged = (newRowSelectionModel: GridRowSelectionModel) => {
    setSeledctedMembers(newRowSelectionModel);
  };

  const handleDeactivateMembersClick = () => {
    const toastId = themedToast(<NotificationContent content={'Deactivating members...'} />, infoNotificationOptions);

    commitChangeOrganizationMembersStatus({
      variables: {
        input: {
          clientMutationId: nanoid(),
          ids: seledctedMembers.map((id) => id as string),
          status: 'Inactive',
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to deactivate members. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Members deactivated.'} />,
        });
        setSeledctedMembers([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to deactivate members. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleActivateMembersClick = () => {
    const toastId = themedToast(<NotificationContent content={'Activating members...'} />, infoNotificationOptions);

    commitChangeOrganizationMembersStatus({
      variables: {
        input: {
          clientMutationId: nanoid(),
          ids: seledctedMembers.map((id) => id as string),
          status: 'Active',
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to activate members. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Members activateed.'} />,
        });
        setSeledctedMembers([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to activate members. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveMembersClick = () => {
    const toastId = themedToast(<NotificationContent content={'Removing members...'} />, infoNotificationOptions);

    commitRemoveOrganizationMembersInput({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          ids: seledctedMembers.map((id) => id as string),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove members. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={'Members removed.'} />,
        });
        setSeledctedMembers([]);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove members. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const rows: RowType[] = members.map((member) => ({
    id: member.id,
    avatar: member.customer,
    name: getCustomerFullName(member.customer),
    teams: member.teams.map((team) => team.name).join(', '),
    email: member.customer.email,
    phoneNumber: member.customer.phoneNumber,
    status: member.status === 'Active',
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
      renderCell: (params) => (
        <StackRow>
          {params.value && (
            <StackRow sx={{ flexWrap: undefined }}>
              <SmallIconTypography label="Active" />
              <Box sx={{ width: 15, height: 15, borderRadius: '50%', backgroundColor: emerald }} />
            </StackRow>
          )}
          {!params.value && (
            <StackRow sx={{ flexWrap: undefined }}>
              <SmallIconTypography label="Deactive" />
              <Box sx={{ width: 15, height: 15, borderRadius: '50%', backgroundColor: flame }} />
            </StackRow>
          )}
        </StackRow>
      ),
      display: 'flex',
    },
  ];

  return (
    <Box sx={{ display: 'flex' }}>
      <OrganizationMembersLeftSideNavigationMenuContent organizationId={organizationId} hideIcons />
      <Box sx={{ marginLeft: expandedDrawerWidthPx, flexGrow: 1 }}>
        <StackColumn sx={{ maxWidth: maxScreenWidth }}>
          <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
            <SectionIconTypography label="Organization Members" />
            <BodyIconTypography label="View members in your organization" />
            <Divider />
          </StackColumn>

          <StackRow sx={{ padding: defaultPadding }}>
            <TeamSelector rootDataRelay={rootData} onChange={handlTeamChanged} />
            <PushToRight />
            <Search size="small" placeholder="Search for members" defaultValue={peopleNameSearchText} onChange={handleSearchTextChange} />
          </StackRow>

          <StackRow sx={{ padding: defaultPadding }}>
            <Box
              sx={{
                backgroundColor: (theme) => theme.palette.background.paper,
                padding: defaultPadding,
                border: 1,
                borderColor: (theme) => theme.palette.divider,
                borderRadius: 2,
                flexGrow: 1,
              }}
            >
              <StackRow sx={{ alignItems: 'center' }}>
                <SmallIconTypography label={`${seledctedMembers.length} records selected`} />
                <PushToRight />
                <Button
                  size="medium"
                  variant="contained"
                  color="secondary"
                  disabled={seledctedMembers.length === 0}
                  onClick={handleDeactivateMembersClick}
                >
                  Deactuvate Member
                </Button>
                <Button
                  size="medium"
                  variant="contained"
                  color="secondary"
                  disabled={seledctedMembers.length === 0}
                  onClick={handleActivateMembersClick}
                >
                  Activate Member
                </Button>
                <Button
                  size="medium"
                  variant="contained"
                  color="warning"
                  startIcon={<DeleteIcon />}
                  disabled={seledctedMembers.length === 0}
                  onClick={handleRemoveMembersClick}
                >
                  Remove Member
                </Button>
              </StackRow>
            </Box>
          </StackRow>

          <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
            <DataGrid
              checkboxSelection
              rowSelectionModel={seledctedMembers}
              onRowSelectionModelChange={handleSelectedMembersChanged}
              rows={rows}
              columns={columns}
              hideFooterPagination={rows.length <= 10}
              initialState={{
                pagination: {
                  rowCount: rows.length,
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
              sx={defaultGridStyle}
            />
          </StackColumn>
        </StackColumn>
      </Box>
    </Box>
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
