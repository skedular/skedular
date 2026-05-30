import { PaletteModeContext, RelayError, getCustomerFullName, getRelayErrorMessage, toRootError } from '@skedular/shared';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';

import type { notifications_acceptInvitationToJoinOrganizationMutation } from '@/queries/__generated__/notifications_acceptInvitationToJoinOrganizationMutation.graphql';
import type { notifications_acceptInvitationToJoinTeamMutation } from '@/queries/__generated__/notifications_acceptInvitationToJoinTeamMutation.graphql';
import type { notifications_rejectInvitationToJoinOrganizationMutation } from '@/queries/__generated__/notifications_rejectInvitationToJoinOrganizationMutation.graphql';
import type { notifications_rejectInvitationToJoinTeamMutation } from '@/queries/__generated__/notifications_rejectInvitationToJoinTeamMutation.graphql';
import type { notifications_rootQuery } from '@/queries/__generated__/notifications_rootQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import Divider from '@mui/material/Divider';

import { defaultButtonStyle, defaultPadding, LeadIconTypography, PageHeaderPanel, SmallIconTypography, StackColumn, StackRow } from '@skedular/ui';
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

  const handleRejectInvitationToJoinOrganizationClick = (id: string) => {
    const invitation = myInvitationsToJoinOrganizations.find((item) => item.id === id);
    if (!invitation) {
      return;
    }

    commitRejectInvitationToJoinOrganization({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: invitation.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(
            <NotificationContent content={`We couldn't reject the invitation to join organization '${invitation.organization?.name}'. ${getRelayErrorMessage(errors)}`} />,
            errorNotificationOptions,
          );

          return;
        }
      },
      onError: (error) => {
        themedToast(
          <NotificationContent content={`We couldn't reject the invitation to join organization '${invitation.organization?.name}'. ${error.message}`} />,
          errorNotificationOptions,
        );
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

    commitAcceptInvitationToJoinOrganization({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(
            <NotificationContent content={`We couldn't accept the invitation to join organization '${invitation.organization?.name}'. ${getRelayErrorMessage(errors)}`} />,
            errorNotificationOptions,
          );

          return;
        }
      },
      onError: (error) => {
        themedToast(
          <NotificationContent content={`We couldn't accept the invitation to join organization '${invitation.organization?.name}'. ${error.message}`} />,
          errorNotificationOptions,
        );
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

    commitRejectInvitationToJoinTeam({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(
            <NotificationContent content={`We couldn't reject the invitation to join team '${invitation.team?.name}'. ${getRelayErrorMessage(errors)}`} />,
            errorNotificationOptions,
          );

          return;
        }
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't reject the invitation to join team '${invitation.team?.name}'. ${error.message}`} />, errorNotificationOptions);
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

    commitAcceptInvitationToJoinTeam({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(
            <NotificationContent content={`We couldn't accept the invitation to join team '${invitation.team?.name}'. ${getRelayErrorMessage(errors)}`} />,
            errorNotificationOptions,
          );

          return;
        }
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't accept the invitation to join team '${invitation.team?.name}'. ${error.message}`} />, errorNotificationOptions);
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

  const pendingInvitationCount = myInvitationsToJoinOrganizations.length + myInvitationsToJoinTeams.length;

  return (
    <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', px: { xs: 0, sm: 1, md: 2 }, pt: { xs: 1, sm: 1, md: 2 }, pb: defaultPadding }}>
      <StackColumn
        sx={{
          width: '100%',
          maxWidth: 1280,
          mx: 'auto',
          backgroundColor: 'transparent',
          gap: 2,
        }}
      >
        <PageHeaderPanel eyebrow="Notifications" title="Notifications" description="Review pending invitations to join organizations and teams.">
          <StackRow sx={{ gap: 1, flexWrap: 'wrap' }}>
            <Chip size="small" label={`${pendingInvitationCount} pending`} />
            <Chip size="small" label={`${myInvitationsToJoinOrganizations.length} organization invitation${myInvitationsToJoinOrganizations.length === 1 ? '' : 's'}`} />
            <Chip size="small" label={`${myInvitationsToJoinTeams.length} team invitation${myInvitationsToJoinTeams.length === 1 ? '' : 's'}`} />
          </StackRow>
        </PageHeaderPanel>

        <Box
          sx={{
            borderRadius: 4,
            border: 1,
            borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : 'divider'),
            bgcolor: (theme) => (theme.palette.mode === 'light' ? 'common.white' : theme.palette.background.paper),
            boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 12px 32px rgba(15, 23, 42, 0.08)' : theme.shadows[1]),
            overflow: 'hidden',
            p: defaultPadding,
          }}
        >
          <StackColumn spacing={2}>
            <StackColumn spacing={0.5}>
              <LeadIconTypography label="Pending Invitations" />
              <SmallIconTypography label="Accept or reject outstanding organization and team invitations." />
            </StackColumn>

            <Divider />

            {pendingInvitationCount === 0 && (
              <StackColumn spacing={0.5}>
                <LeadIconTypography label="No pending notifications" />
                <SmallIconTypography label="You do not have any organization or team invitations to review." />
              </StackColumn>
            )}

            {myInvitationsToJoinOrganizations.length > 0 && (
              <StackColumn spacing={1.5}>
                <LeadIconTypography label="Organization Invitations" />
                {myInvitationsToJoinOrganizations.map((invitation, index) => (
                  <StackColumn key={invitation.id} spacing={1.5}>
                    {index > 0 && <Divider />}
                    <StackRow sx={{ alignItems: 'center', justifyContent: 'space-between', gap: 2, flexWrap: 'wrap' }}>
                      <StackColumn spacing={0.5}>
                        <SmallIconTypography label={`"${getCustomerFullName(invitation.createdBy)}" has invited you to join organization "${invitation.organization.name}"`} />
                        <SmallIconTypography label={`Status: ${invitation.status.name}`} />
                      </StackColumn>

                      {invitation.status.type === 'PENDING' && (
                        <StackRow sx={{ gap: 1, flexWrap: 'wrap' }}>
                          <Button variant="contained" color="secondary" onClick={() => handleRejectInvitationToJoinOrganizationClick(invitation.id)} sx={defaultButtonStyle}>
                            Reject
                          </Button>
                          <Button variant="contained" onClick={() => handleAcceptInvitationToJoinOrganizationClick(invitation.id)} sx={defaultButtonStyle}>
                            Accept
                          </Button>
                        </StackRow>
                      )}
                    </StackRow>
                  </StackColumn>
                ))}
              </StackColumn>
            )}

            {myInvitationsToJoinOrganizations.length > 0 && myInvitationsToJoinTeams.length > 0 && <Divider />}

            {myInvitationsToJoinTeams.length > 0 && (
              <StackColumn spacing={1.5}>
                <LeadIconTypography label="Team Invitations" />
                {myInvitationsToJoinTeams.map((invitation, index) => (
                  <StackColumn key={invitation.id} spacing={1.5}>
                    {index > 0 && <Divider />}
                    <StackRow sx={{ alignItems: 'center', justifyContent: 'space-between', gap: 2, flexWrap: 'wrap' }}>
                      <StackColumn spacing={0.5}>
                        <SmallIconTypography label={`"${getCustomerFullName(invitation.createdBy)}" has invited you to join team "${invitation.team.name}"`} />
                        <SmallIconTypography label={`Status: ${invitation.status.name}`} />
                      </StackColumn>

                      {invitation.status.type === 'PENDING' && (
                        <StackRow sx={{ gap: 1, flexWrap: 'wrap' }}>
                          <Button variant="contained" color="secondary" onClick={() => handleRejectInvitationToJoinTeamClick(invitation.id)} sx={defaultButtonStyle}>
                            Reject
                          </Button>
                          <Button variant="contained" onClick={() => handleAcceptInvitationToJoinTeamClick(invitation.id)} sx={defaultButtonStyle}>
                            Accept
                          </Button>
                        </StackRow>
                      )}
                    </StackRow>
                  </StackColumn>
                ))}
              </StackColumn>
            )}
          </StackColumn>
        </Box>
      </StackColumn>
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
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoNotifications queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(NotificationsWithRelay);
