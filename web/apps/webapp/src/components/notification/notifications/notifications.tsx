import { AppBarWithStackColumn, SmallIconTypography, StackColumn, StackRow } from '@/components/commons';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { PaletteModeContext } from '@/libs/providers';
import { defaultGridStyle, defaultPadding, maxScreenWidth } from '@/libs/theme';
import { getCustomerFullName, joinErrors } from '@/libs/utils';
import type { notifications_acceptInvitationToJoinLocationMutation } from '@/queries/__generated__/notifications_acceptInvitationToJoinLocationMutation.graphql';
import type { notifications_acceptInvitationToJoinOrganizationMutation } from '@/queries/__generated__/notifications_acceptInvitationToJoinOrganizationMutation.graphql';
import type { notifications_acceptInvitationToJoinTeamMutation } from '@/queries/__generated__/notifications_acceptInvitationToJoinTeamMutation.graphql';
import type { notifications_rejectInvitationToJoinLocationMutation } from '@/queries/__generated__/notifications_rejectInvitationToJoinLocationMutation.graphql';
import type { notifications_rejectInvitationToJoinOrganizationMutation } from '@/queries/__generated__/notifications_rejectInvitationToJoinOrganizationMutation.graphql';
import type { notifications_rejectInvitationToJoinTeamMutation } from '@/queries/__generated__/notifications_rejectInvitationToJoinTeamMutation.graphql';
import type { notifications_rootQuery } from '@/queries/__generated__/notifications_rootQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import type { GridColDef } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import { nanoid } from 'nanoid';
import { useRouter } from 'next/navigation';
import { memo, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';

type Props = {
  queryReference: PreloadedQuery<notifications_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query notifications_rootQuery($myNotificationsSortingValues: [NotificationOrderInput!]) {
    myNotifications(where: {}, orderBy: $myNotificationsSortingValues) {
      __id
      totalCount
      edges {
        node {
          id
          sourceId
          notificationType
          invitedBy {
            name
            givenName
            middleName
            familyName
            photoUrl
          }
          organization {
            name
          }
          location {
            name
          }
          team {
            name
          }
        }
      }
    }
  }
`;

type RowType = {
  id: string;
  changeTriggerId?: string;
};

const Notifications = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<notifications_rootQuery>(RootQuery, queryReference);

  const [commitAcceptInvitationToJoinOrganization] = useMutation<notifications_acceptInvitationToJoinOrganizationMutation>(graphql`
    mutation notifications_acceptInvitationToJoinOrganizationMutation($input: AcceptInvitationToJoinOrganizationInput!) {
      acceptInvitationToJoinOrganization(input: $input) {
        clientMutationId
      }
    }
  `);

  const [commitRejectInvitationToJoinOrganization] = useMutation<notifications_rejectInvitationToJoinOrganizationMutation>(graphql`
    mutation notifications_rejectInvitationToJoinOrganizationMutation($input: RejectInvitationToJoinOrganizationInput!) {
      rejectInvitationToJoinOrganization(input: $input) {
        clientMutationId
      }
    }
  `);

  const [commitAcceptInvitationToJoinLocation] = useMutation<notifications_acceptInvitationToJoinLocationMutation>(graphql`
    mutation notifications_acceptInvitationToJoinLocationMutation($input: AcceptInvitationToJoinLocationInput!) {
      acceptInvitationToJoinLocation(input: $input) {
        clientMutationId
      }
    }
  `);

  const [commitRejectInvitationToJoinLocation] = useMutation<notifications_rejectInvitationToJoinLocationMutation>(graphql`
    mutation notifications_rejectInvitationToJoinLocationMutation($input: RejectInvitationToJoinLocationInput!) {
      rejectInvitationToJoinLocation(input: $input) {
        clientMutationId
      }
    }
  `);

  const [commitAcceptInvitationToJoinTeam] = useMutation<notifications_acceptInvitationToJoinTeamMutation>(graphql`
    mutation notifications_acceptInvitationToJoinTeamMutation($input: AcceptInvitationToJoinTeamInput!) {
      acceptInvitationToJoinTeam(input: $input) {
        clientMutationId
      }
    }
  `);

  const [commitRejectInvitationToJoinTeam] = useMutation<notifications_rejectInvitationToJoinTeamMutation>(graphql`
    mutation notifications_rejectInvitationToJoinTeamMutation($input: RejectInvitationToJoinTeamInput!) {
      rejectInvitationToJoinTeam(input: $input) {
        clientMutationId
      }
    }
  `);

  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [rejectedIds, setRejectedIds] = useState<string[]>([]);
  const [acceptedIds, setAcceptedIds] = useState<string[]>([]);
  const myNotifications = useMemo(() => (rootData.myNotifications ? rootData.myNotifications.edges.map((edge) => edge.node) : []), [rootData.myNotifications]);

  const handleCloseClick = () => {
    router.back();
  };

  const handleRejectInvitationToJoinOrganizationClick = (id: string) => {
    const notification = myNotifications.find((item) => item.id === id);
    if (!notification) {
      return <></>;
    }

    const toastId = themedToast(<NotificationContent content={`Rejecting invitation to join organization '${notification.organization?.name}'...`} />, infoNotificationOptions);

    commitRejectInvitationToJoinOrganization({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: notification.sourceId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to reject invitation to join organization '${notification.organization?.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Invitation to join organization '${notification.organization?.name} rejected.`} />,
        });

        setRejectedIds(rejectedIds.concat(id));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to reject invitation to join organization '${notification.organization?.name}'. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleAcceptInvitationToJoinOrganizationClick = (id: string) => {
    const notification = myNotifications.find((item) => item.id === id);
    if (!notification) {
      return <></>;
    }

    const toastId = themedToast(<NotificationContent content={`Accpeting invitation to join organization '${notification.organization?.name}'...`} />, infoNotificationOptions);

    commitAcceptInvitationToJoinOrganization({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: notification.sourceId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to accept invitation to join organization '${notification.organization?.name}'. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Invitation to join organization '${notification.organization?.name} accepted.`} />,
        });

        setAcceptedIds(acceptedIds.concat(id));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to accept invitation to join organization '${notification.organization?.name}'. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRejectInvitationToJoinLocationClick = (id: string) => {
    const notification = myNotifications.find((item) => item.id === id);
    if (!notification) {
      return <></>;
    }

    const toastId = themedToast(<NotificationContent content={`Rejecting invitation to join location '${notification.location?.name}'...`} />, infoNotificationOptions);

    commitRejectInvitationToJoinLocation({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: notification.sourceId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to reject invitation to join location '${notification.location?.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Invitation to join location '${notification.location?.name} rejected.`} />,
        });

        setRejectedIds(rejectedIds.concat(id));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to reject invitation to join location '${notification.location?.name}'. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleAcceptInvitationToJoinLocationClick = (id: string) => {
    const notification = myNotifications.find((item) => item.id === id);
    if (!notification) {
      return <></>;
    }

    const toastId = themedToast(<NotificationContent content={`Accpeting invitation to join location '${notification.location?.name}'...`} />, infoNotificationOptions);

    commitAcceptInvitationToJoinLocation({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: notification.sourceId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to accept invitation to join location '${notification.location?.name}'. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Invitation to join location '${notification.location?.name} accepted.`} />,
        });

        setAcceptedIds(acceptedIds.concat(id));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to accept invitation to join location '${notification.location?.name}'. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRejectInvitationToJoinTeamClick = (id: string) => {
    const notification = myNotifications.find((item) => item.id === id);
    if (!notification) {
      return <></>;
    }

    const toastId = themedToast(<NotificationContent content={`Rejecting invitation to join team '${notification.team?.name}'...`} />, infoNotificationOptions);

    commitRejectInvitationToJoinTeam({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: notification.sourceId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to reject invitation to join team '${notification.team?.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Invitation to join team '${notification.team?.name} rejected.`} />,
        });

        setRejectedIds(rejectedIds.concat(id));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to reject invitation to join team '${notification.team?.name}'. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleAcceptInvitationToJoinTeamClick = (id: string) => {
    const notification = myNotifications.find((item) => item.id === id);
    if (!notification) {
      return <></>;
    }

    const toastId = themedToast(<NotificationContent content={`Accpeting invitation to join team '${notification.team?.name}'...`} />, infoNotificationOptions);

    commitAcceptInvitationToJoinTeam({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: notification.sourceId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to accept invitation to join team '${notification.team?.name}'. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Invitation to join team '${notification.team?.name} accepted.`} />,
        });

        setAcceptedIds(acceptedIds.concat(id));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to accept invitation to join team '${notification.team?.name}'. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const rows: RowType[] = useMemo(
    () =>
      myNotifications.map((notification) => {
        const rejectedOrAccepted = rejectedIds.includes(notification.id) || acceptedIds.includes(notification.id);

        return {
          id: notification.id,
          changeTriggerId: rejectedOrAccepted ? nanoid() : undefined,
        };
      }),
    [myNotifications, rejectedIds, acceptedIds],
  );

  const columns: GridColDef<(typeof rows)[number]>[] = [
    {
      field: 'message',
      headerName: '',
      editable: false,
      renderCell: (params) => {
        const id = params.id as string;
        const notification = myNotifications.find((item) => item.id === id);
        if (!notification) {
          return <></>;
        }

        switch (notification.notificationType) {
          case 'InvitationToJoinOrganization':
            return <SmallIconTypography label={`"${getCustomerFullName(notification.invitedBy)}" has invited you to join organization "${notification.organization?.name}"`} />;

          case 'InvitationToJoinLocation':
            return <SmallIconTypography label={`"${getCustomerFullName(notification.invitedBy)}" has invited you to join location "${notification.location?.name}"`} />;

          case 'InvitationToJoinTeam':
            return <SmallIconTypography label={`"${getCustomerFullName(notification.invitedBy)}" has invited you to join team "${notification.team?.name}"`} />;

          default:
            return <></>;
        }
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
        const notification = myNotifications.find((item) => item.id === (params.id as string));
        if (!notification) {
          return <></>;
        }

        if (rejectedIds.includes(id)) {
          return <SmallIconTypography label="Rejected" />;
        }

        if (acceptedIds.includes(id)) {
          return <SmallIconTypography label="Accepted" />;
        }

        switch (notification.notificationType) {
          case 'InvitationToJoinOrganization':
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

          case 'InvitationToJoinLocation':
            return (
              <StackRow sx={{ paddingTop: 1, paddingBottom: 1 }}>
                <Button variant="contained" color="secondary" onClick={() => handleRejectInvitationToJoinLocationClick(id)} sx={{ textTransform: 'none' }}>
                  <SmallIconTypography label="Reject" />
                </Button>
                <Button variant="contained" onClick={() => handleAcceptInvitationToJoinLocationClick(id)} sx={{ textTransform: 'none' }}>
                  <SmallIconTypography label="Approve" />
                </Button>
              </StackRow>
            );

          case 'InvitationToJoinTeam':
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

          default:
            return <></>;
        }
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
          <StackRow sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
            <DataGrid
              rows={rows}
              columns={columns}
              ignoreDiacritics
              disableRowSelectionOnClick
              hideFooter
              getRowHeight={() => 'auto'}
              rowSpacingType="margin"
              getRowSpacing={() => ({ top: 3, bottom: 3 })}
              sx={defaultGridStyle}
              localeText={{ noRowsLabel: 'No notification found' }}
            />
          </StackRow>
        </StackColumn>
      </Box>
    </Box>
  );
};

const MemoNotifications = memo(Notifications);

const NotificationsWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<notifications_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        myNotificationsSortingValues: [
          {
            direction: 'Descending',
            field: 'Date',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId]);

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
      <MemoNotifications queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(NotificationsWithRelay);
