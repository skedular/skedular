import { GridContainer, LeadIconTypography, SectionIconTypography, SmallIconTypography, StackColumn } from '@/components/commons';
import { CancelIcon, InviteMemberIcon, LocationIcon, ResourceIcon, TeamIcon } from '@/components/icons';
import { getOrganizationLocationAddLink, getOrganizationLocationManageResourcesBaseLink, getOrganizationTeamAddLink, getOrganizationUsersBaseLink } from '@/components/links';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { InvitePeopleToJoinOrganizationDialog } from '@/components/organization/invitePeopleToJoinOrganization';
import { AddResourceDialog } from '@/components/resource/addResource';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { defaultPadding, emerald } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { gettingStarted_completeOrganizationMemberOnboardingMutation } from '@/queries/__generated__/gettingStarted_completeOrganizationMemberOnboardingMutation.graphql';
import type { gettingStarted_query$key } from '@/queries/__generated__/gettingStarted_query.graphql';
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Paper from '@mui/material/Paper';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, useContext, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  rootDataRelay: gettingStarted_query$key;
  onReloadRequired: () => void;
  organizationId: string;
};

const GettingStarted = ({ rootDataRelay, onReloadRequired, organizationId }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment gettingStarted_query on Query {
        organization(id: $organizationId) {
          isMyOnboardingDone
        }
      }
    `,
    rootDataRelay,
  );

  const [commitCompleteOrganizationMemberOnboarding] = useMutation<gettingStarted_completeOrganizationMemberOnboardingMutation>(graphql`
    mutation gettingStarted_completeOrganizationMemberOnboardingMutation($input: CompleteOrganizationMemberOnboardingInput!) {
      completeOrganizationMemberOnboarding(input: $input) {
        clientMutationId
      }
    }
  `);

  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [isInvitePeopleToJoinOrganizationDialogOpen, setIsInvitePeopleToJoinOrganizationDialogOpen] = useState(false);
  const [isAddResourceDialogOpen, setIsAddResourceDialogOpen] = useState(false);

  const handleAddResourcesClicked = () => {
    setIsAddResourceDialogOpen(true);
  };

  const handleAddResourceClicked = (locationId: string) => {
    setIsAddResourceDialogOpen(false);

    router.push(getOrganizationLocationManageResourcesBaseLink(integratedPlatrform, organizationId, locationId));
  };

  const handleCancelAddResourceClicked = () => {
    setIsAddResourceDialogOpen(false);
  };

  const handleInviteTeammatesClicked = () => {
    setIsInvitePeopleToJoinOrganizationDialogOpen(true);
  };

  const handleInvitePeopleToJoinOrganizationClicked = () => {
    setIsInvitePeopleToJoinOrganizationDialogOpen(false);

    router.push(getOrganizationUsersBaseLink(integratedPlatrform, organizationId));
  };

  const handleInvitePeopleToJoinOrganizationCancelClicked = () => {
    setIsInvitePeopleToJoinOrganizationDialogOpen(false);
  };

  const handleDismissButtonClicked = () => {
    commitCompleteOrganizationMemberOnboarding({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`Failed to dismiss organization onboarding. Error: ${joinErrors(errors)}.`} />, errorNotificationOptions);
        }

        onReloadRequired();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to dismiss organization onboarding. Error: ${error.message}}.`} />, errorNotificationOptions);
      },
    });
  };

  if (!rootData.organization || rootData.organization.isMyOnboardingDone) {
    return <></>;
  }

  return (
    <>
      <Box sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
        <Box
          sx={{
            border: 2,
            borderRadius: 4,
            borderColor: 'gray',
            borderStyle: 'dashed',
            padding: defaultPadding,
            position: 'relative', // Ensure the parent container is relative, this is to make sure the CancelIcon can be placed to the top right corner of the box
          }}
        >
          <Box sx={{ position: 'absolute', top: 1, right: 1 }}>
            <IconButton onClick={handleDismissButtonClicked} color="inherit">
              <CancelIcon fontSize="large" />
            </IconButton>
          </Box>

          <SectionIconTypography label="Getting started" />

          <GridContainer spacing={1} sx={{ alignItems: 'center', paddingTop: 2, justifyContent: 'space-between' }}>
            <Grid>
              <StackColumn sx={{ width: 250 }}>
                <SmallIconTypography label="Let's start by setting up the organization's first location." />
                <Link component={NextLink} href={getOrganizationLocationAddLink(integratedPlatrform, organizationId)}>
                  <Paper sx={{ height: 100, borderRadius: 2, '&:hover': { border: 1, borderColor: emerald } }}>
                    <LeadIconTypography
                      label="Create Location"
                      stackMode="column"
                      startElement={<LocationIcon fontSize="large" excludeTooltip sx={{ color: emerald }} />}
                      sx={{ alignItems: 'center', justifyContent: 'center', height: '100%' }}
                    />
                  </Paper>
                </Link>
              </StackColumn>
            </Grid>

            <Grid>
              <StackColumn sx={{ width: 250 }}>
                <SmallIconTypography label="Create teams that regularly work or meet together." />
                <Link component={NextLink} href={getOrganizationTeamAddLink(integratedPlatrform, organizationId)}>
                  <Paper sx={{ height: 100, borderRadius: 2, '&:hover': { border: 1, borderColor: emerald } }}>
                    <LeadIconTypography
                      label="Create Team"
                      stackMode="column"
                      startElement={<TeamIcon fontSize="large" excludeTooltip sx={{ color: emerald }} />}
                      sx={{ alignItems: 'center', justifyContent: 'center', height: '100%' }}
                    />
                  </Paper>
                </Link>
              </StackColumn>
            </Grid>

            <Grid>
              <StackColumn sx={{ width: 250 }}>
                <SmallIconTypography label="Add resources for your locations and teams." />
                <Paper sx={{ height: 100, borderRadius: 2, '&:hover': { border: 1, borderColor: emerald } }} onClick={handleAddResourcesClicked}>
                  <LeadIconTypography
                    label="Add Resources"
                    stackMode="column"
                    startElement={<ResourceIcon fontSize="large" excludeTooltip sx={{ color: emerald }} />}
                    sx={{ alignItems: 'center', justifyContent: 'center', height: '100%' }}
                  />
                </Paper>
              </StackColumn>
            </Grid>

            <Grid>
              <StackColumn sx={{ width: 250 }}>
                <SmallIconTypography label="Invite your team members to your organization and start booking!" />
                <Paper sx={{ height: 100, borderRadius: 2, '&:hover': { border: 1, borderColor: emerald } }} onClick={handleInviteTeammatesClicked}>
                  <LeadIconTypography
                    label="Invite Teammates"
                    stackMode="column"
                    startElement={<InviteMemberIcon fontSize="large" sx={{ color: emerald }} />}
                    sx={{ alignItems: 'center', justifyContent: 'center', height: '100%' }}
                  />
                </Paper>
              </StackColumn>
            </Grid>
          </GridContainer>
        </Box>
      </Box>

      <AddResourceDialog
        onReloadRequired={onReloadRequired}
        organizationId={organizationId}
        connectionIds={[]}
        isDialogOpen={isAddResourceDialogOpen}
        onAddClicked={handleAddResourceClicked}
        onCancel={handleCancelAddResourceClicked}
      />

      <InvitePeopleToJoinOrganizationDialog
        isDialogOpen={isInvitePeopleToJoinOrganizationDialogOpen}
        onInviteClicked={handleInvitePeopleToJoinOrganizationClicked}
        onCancel={handleInvitePeopleToJoinOrganizationCancelClicked}
        organizationId={organizationId}
      />
    </>
  );
};

export default memo(GettingStarted);
