import { AppBarWithStackColumn, SmallIconTypography, StackColumn, StackRow } from '@/components/commons';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { PaletteModeContext } from '@/libs/providers';
import { defaultGridStyle, defaultPadding, maxScreenWidth } from '@/libs/theme';
import { getCustomerFullName, joinErrors } from '@/libs/utils';
import type { notifications_acceptInvitationToJoinOrganizationMutation } from '@/queries/__generated__/notifications_acceptInvitationToJoinOrganizationMutation.graphql';
import type { notifications_acceptInvitationToJoinTeamMutation } from '@/queries/__generated__/notifications_acceptInvitationToJoinTeamMutation.graphql';
import type { notifications_rejectInvitationToJoinOrganizationMutation } from '@/queries/__generated__/notifications_rejectInvitationToJoinOrganizationMutation.graphql';
import type { notifications_rejectInvitationToJoinTeamMutation } from '@/queries/__generated__/notifications_rejectInvitationToJoinTeamMutation.graphql';
import type { notifications_rootQuery } from '@/queries/__generated__/notifications_rootQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import type { GridColDef } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import { useRouter } from 'next/navigation';
import { memo, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  queryReference: PreloadedQuery<notifications_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query notifications_rootQuery {
    organizationInvitationStatuses {
      type
      name
    }
    myInvitationsToJoinOrganizations(where: { status: PENDING }, orderBy: [{ field: CREATED_AT, direction: ASCENDING }]) {
      __id
      totalCount
      edges {
        node {
          id
          status {
            type
            name
          }
          createdBy {
            name
            givenName
            middleName
            familyName
            photoUrl
          }
          organization {
            name
          }
        }
      }
    }
    teamInvitationStatuses {
      type
      name
    }
    myInvitationsToJoinTeams(where: { status: PENDING }, orderBy: [{ field: CREATED_AT, direction: ASCENDING }]) {
      __id
      totalCount
      edges {
        node {
          id
          status {
            type
            name
          }
          createdBy {
            name
            givenName
            middleName
            familyName
            photoUrl
          }
          team {
            name
          }
        }
      }
    }
  }
`;

type InvitationToJoinOrganizationRowType = {
  id: string;
  changeTriggerId?: string;
};

type InvitationToJoinTeamRowType = {
  id: string;
};

const Notifications = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<notifications_rootQuery>(RootQuery, queryReference);

  const [commitAcceptInvitationToJoinOrganization] = useMutation<notifications_acceptInvitationToJoinOrganizationMutation>(graphql`
    mutation notifications_acceptInvitationToJoinOrganizationMutation($input: AcceptInvitationToJoinOrganizationInput!) @raw_response_type {
      acceptInvitationToJoinOrganization(input: $input) {
        inviteCustomerToJoinOrganization {
          id
          status {
            type
            name
          }
        }
      }
    }
  `);

  const [commitRejectInvitationToJoinOrganization] = useMutation<notifications_rejectInvitationToJoinOrganizationMutation>(graphql`
    mutation notifications_rejectInvitationToJoinOrganizationMutation($input: RejectInvitationToJoinOrganizationInput!) @raw_response_type {
      rejectInvitationToJoinOrganization(input: $input) {
        inviteCustomerToJoinOrganization {
          id
          status {
            type
            name
          }
        }
      }
    }
  `);

  const [commitAcceptInvitationToJoinTeam] = useMutation<notifications_acceptInvitationToJoinTeamMutation>(graphql`
    mutation notifications_acceptInvitationToJoinTeamMutation($input: AcceptInvitationToJoinTeamInput!) @raw_response_type {
      acceptInvitationToJoinTeam(input: $input) {
        inviteCustomerToJoinTeam {
          id
          status {
            type
            name
          }
        }
      }
    }
  `);

  const [commitRejectInvitationToJoinTeam] = useMutation<notifications_rejectInvitationToJoinTeamMutation>(graphql`
    mutation notifications_rejectInvitationToJoinTeamMutation($input: RejectInvitationToJoinTeamInput!) @raw_response_type {
      rejectInvitationToJoinTeam(input: $input) {
        inviteCustomerToJoinTeam {
          id
          status {
            type
            name
          }
        }
      }
    }
  `);

  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const myInvitationsToJoinOrganizations = useMemo(
    () => (rootData.myInvitationsToJoinOrganizations ? rootData.myInvitationsToJoinOrganizations.edges.map((edge) => edge.node) : []),
    [rootData.myInvitationsToJoinOrganizations],
  );
  const myInvitationsToJoinTeams = useMemo(
    () => (rootData.myInvitationsToJoinTeams ? rootData.myInvitationsToJoinTeams.edges.map((edge) => edge.node) : []),
    [rootData.myInvitationsToJoinTeams],
  );

  const handleCloseClick = () => {
    router.back();
  };

  const handleRejectInvitationToJoinOrganizationClick = (id: string) => {
    const invitation = myInvitationsToJoinOrganizations.find((item) => item.id === id);
    if (!invitation) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Rejecting invitation to join organization '${invitation.organization?.name}'...`} />, infoNotificationOptions);

    commitRejectInvitationToJoinOrganization({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: invitation.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to reject invitation to join organization '${invitation.organization?.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Invitation to join organization '${invitation.organization?.name} rejected.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to reject invitation to join organization '${invitation.organization?.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        rejectInvitationToJoinOrganization: {
          inviteCustomerToJoinOrganization: {
            id,
            status: {
              type: 'REJECTED',
              name: rootData.organizationInvitationStatuses.find((item) => item.type === 'REJECTED')!.name,
            },
          },
        },
      },
    });
  };

  const handleAcceptInvitationToJoinOrganizationClick = (id: string) => {
    const invitation = myInvitationsToJoinOrganizations.find((item) => item.id === id);
    if (!invitation) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Accpeting invitation to join organization '${invitation.organization?.name}'...`} />, infoNotificationOptions);

    commitAcceptInvitationToJoinOrganization({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to accept invitation to join organization '${invitation.organization?.name}'. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Invitation to join organization '${invitation.organization?.name} accepted.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to accept invitation to join organization '${invitation.organization?.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        acceptInvitationToJoinOrganization: {
          inviteCustomerToJoinOrganization: {
            id,
            status: {
              type: 'ACCEPTED',
              name: rootData.organizationInvitationStatuses.find((item) => item.type === 'ACCEPTED')!.name,
            },
          },
        },
      },
    });
  };

  const handleRejectInvitationToJoinTeamClick = (id: string) => {
    const invitation = myInvitationsToJoinTeams.find((item) => item.id === id);
    if (!invitation) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Rejecting invitation to join team '${invitation.team?.name}'...`} />, infoNotificationOptions);

    commitRejectInvitationToJoinTeam({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to reject invitation to join team '${invitation.team?.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Invitation to join team '${invitation.team?.name} rejected.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to reject invitation to join team '${invitation.team?.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        rejectInvitationToJoinTeam: {
          inviteCustomerToJoinTeam: {
            id,
            status: {
              type: 'REJECTED',
              name: rootData.teamInvitationStatuses.find((item) => item.type === 'REJECTED')!.name,
            },
          },
        },
      },
    });
  };

  const handleAcceptInvitationToJoinTeamClick = (id: string) => {
    const invitation = myInvitationsToJoinTeams.find((item) => item.id === id);
    if (!invitation) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Accpeting invitation to join team '${invitation.team?.name}'...`} />, infoNotificationOptions);

    commitAcceptInvitationToJoinTeam({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to accept invitation to join team '${invitation.team?.name}'. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Invitation to join team '${invitation.team?.name} accepted.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to accept invitation to join team '${invitation.team?.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        acceptInvitationToJoinTeam: {
          inviteCustomerToJoinTeam: {
            id,
            status: {
              type: 'ACCEPTED',
              name: rootData.teamInvitationStatuses.find((item) => item.type === 'ACCEPTED')!.name,
            },
          },
        },
      },
    });
  };

  const invitationsToJoinOrganizationsRows: InvitationToJoinOrganizationRowType[] = useMemo(
    () => myInvitationsToJoinOrganizations.map((invitation) => ({ id: invitation.id })),
    [myInvitationsToJoinOrganizations],
  );

  const invitationsToJoinOrganizationsColumns: GridColDef<(typeof invitationsToJoinOrganizationsRows)[number]>[] = [
    {
      field: 'message',
      headerName: '',
      editable: false,
      renderCell: (params) => {
        const id = params.id as string;
        const invitation = myInvitationsToJoinOrganizations.find((item) => item.id === id);
        if (!invitation) {
          return null;
        }

        return <SmallIconTypography label={`"${getCustomerFullName(invitation.createdBy)}" has invited you to join organization "${invitation.organization.name}"`} />;
      },
      display: 'flex',
      minWidth: 500,
    },
    {
      field: 'rejectOrApprove',
      headerName: '',
      editable: false,
      renderCell: (params) => {
        const id = params.id as string;
        const invitation = myInvitationsToJoinOrganizations.find((item) => item.id === (params.id as string));
        if (!invitation) {
          return null;
        }

        if (invitation.status.type === 'PENDING') {
          return (
            <StackRow sx={{ paddingTop: 1, paddingBottom: 1 }}>
              <Button variant="contained" color="secondary" onClick={() => handleRejectInvitationToJoinOrganizationClick(id)} sx={{ textTransform: 'none' }}>
                <SmallIconTypography label="Reject" />
              </Button>
              <Button variant="contained" onClick={() => handleAcceptInvitationToJoinOrganizationClick(id)} sx={{ textTransform: 'none' }}>
                <SmallIconTypography label="Approve" />
              </Button>
            </StackRow>
          );
        }

        return <SmallIconTypography label={invitation.status.name} />;
      },
      display: 'flex',
      minWidth: 300,
    },
  ];

  const invitationsToJoinTeamsRows: InvitationToJoinTeamRowType[] = useMemo(
    () => myInvitationsToJoinTeams.map((invitation) => ({ id: invitation.id })),
    [myInvitationsToJoinTeams],
  );

  const invitationsToJoinTeamsColumns: GridColDef<(typeof invitationsToJoinTeamsRows)[number]>[] = [
    {
      field: 'message',
      headerName: '',
      editable: false,
      renderCell: (params) => {
        const id = params.id as string;
        const invitation = myInvitationsToJoinTeams.find((item) => item.id === id);
        if (!invitation) {
          return null;
        }

        return <SmallIconTypography label={`"${getCustomerFullName(invitation.createdBy)}" has invited you to join team "${invitation.team.name}"`} />;
      },
      display: 'flex',
      minWidth: 500,
    },
    {
      field: 'rejectOrApprove',
      headerName: '',
      editable: false,
      renderCell: (params) => {
        const id = params.id as string;
        const invitation = myInvitationsToJoinTeams.find((item) => item.id === (params.id as string));
        if (!invitation) {
          return null;
        }

        if (invitation.status.type === 'PENDING') {
          return (
            <StackRow sx={{ paddingTop: 1, paddingBottom: 1 }}>
              <Button variant="contained" color="secondary" onClick={() => handleRejectInvitationToJoinTeamClick(id)} sx={{ textTransform: 'none' }}>
                <SmallIconTypography label="Reject" />
              </Button>
              <Button variant="contained" onClick={() => handleAcceptInvitationToJoinTeamClick(id)} sx={{ textTransform: 'none' }}>
                <SmallIconTypography label="Approve" />
              </Button>
            </StackRow>
          );
        }

        return <SmallIconTypography label={invitation.status.name} />;
      },
      display: 'flex',
      minWidth: 300,
    },
  ];

  return (
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Notifications" />
        <StackColumn sx={{ maxWidth: maxScreenWidth }}>
          {invitationsToJoinOrganizationsRows.length > 0 && (
            <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
              <DataGrid
                rows={invitationsToJoinOrganizationsRows}
                columns={invitationsToJoinOrganizationsColumns}
                ignoreDiacritics
                disableRowSelectionOnClick
                hideFooter
                getRowHeight={() => 'auto'}
                rowSpacingType="margin"
                getRowSpacing={() => ({ top: 3, bottom: 3 })}
                sx={defaultGridStyle}
                localeText={{ noRowsLabel: 'No organization invitation found' }}
              />
            </StackRow>
          )}
          {invitationsToJoinTeamsRows.length > 0 && (
            <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
              <DataGrid
                rows={invitationsToJoinTeamsRows}
                columns={invitationsToJoinTeamsColumns}
                ignoreDiacritics
                disableRowSelectionOnClick
                hideFooter
                getRowHeight={() => 'auto'}
                rowSpacingType="margin"
                getRowSpacing={() => ({ top: 3, bottom: 3 })}
                sx={defaultGridStyle}
                localeText={{ noRowsLabel: 'No team invitation found' }}
              />
            </StackRow>
          )}
        </StackColumn>
      </Box>
    </Box>
  );
};

const MemoNotifications = memo(Notifications);

const NotificationsWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<notifications_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {},
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoNotifications queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(NotificationsWithRelay);
