import { LeadIconTypography, SectionIconTypography, SmallIconTypography, StackColumn } from '@skedular/ui';
import { CancelIcon, InviteMemberIcon, LocationIcon, ResourceIcon, TeamIcon } from '@/components/icons';
import { getOrganizationAddResourceBaseLink, getOrganizationLocationAddPrivateLink, getOrganizationTeamAddLink, getOrganizationUsersBaseLink } from '@/components/links';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { InvitePeopleToJoinOrganizationDialog } from '@/components/organization/invitePeopleToJoinOrganization';
import { PaletteModeContext, useIntegratedPlatrform } from '@skedular/shared';
import { defaultPadding, emerald } from '@skedular/ui';
import { getRelayErrorMessage } from '@skedular/shared';
import type { gettingStarted_completeOrganizationMemberOnboardingMutation } from '@/queries/__generated__/gettingStarted_completeOrganizationMemberOnboardingMutation.graphql';
import type { gettingStarted_query$key } from '@/queries/__generated__/gettingStarted_query.graphql';
import Box from '@mui/material/Box';
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
  organizationCustomDomain: string;
};

const GettingStarted = ({ rootDataRelay, onReloadRequired, organizationCustomDomain }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment gettingStarted_query on Query {
        organization(customDomain: $organizationCustomDomain) {
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

  const handleInviteTeammatesClicked = () => {
    setIsInvitePeopleToJoinOrganizationDialogOpen(true);
  };

  const handleInvitePeopleToJoinOrganizationClicked = () => {
    setIsInvitePeopleToJoinOrganizationDialogOpen(false);

    router.push(getOrganizationUsersBaseLink(integratedPlatrform, organizationCustomDomain));
  };

  const handleInvitePeopleToJoinOrganizationCancelClicked = () => {
    setIsInvitePeopleToJoinOrganizationDialogOpen(false);
  };

  const handleDismissButtonClicked = () => {
    commitCompleteOrganizationMemberOnboarding({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationCustomDomain,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`Failed to dismiss organization onboarding. Error: ${getRelayErrorMessage(errors)}.`} />, errorNotificationOptions);
        }

        onReloadRequired();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to dismiss organization onboarding. Error: ${error.message}}.`} />, errorNotificationOptions);
      },
    });
  };

  if (!rootData.organization || rootData.organization.isMyOnboardingDone) {
    return null;
  }

  return (
    <>
      <Box
        sx={{
          border: 2,
          borderRadius: 4,
          borderColor: 'gray',
          borderStyle: 'dashed',
          padding: defaultPadding,
          position: 'relative',
        }}
      >
        <Box sx={{ position: 'absolute', top: 1, right: 1 }}>
          <IconButton onClick={handleDismissButtonClicked} color="inherit">
            <CancelIcon fontSize="large" />
          </IconButton>
        </Box>

        <SectionIconTypography label="Getting started" />

        <Box
          sx={{
            display: 'grid',
            gridTemplateColumns: {
              xs: '1fr',
              sm: 'repeat(2, minmax(0, 1fr))',
              xl: 'repeat(4, minmax(0, 1fr))',
            },
            gap: 2,
            alignItems: 'start',
            paddingTop: 2,
          }}
        >
          <StackColumn spacing={1} sx={{ minWidth: 0 }}>
            <SmallIconTypography label="Let's start by setting up the organization's first location." />
            <Link component={NextLink} href={getOrganizationLocationAddPrivateLink(integratedPlatrform, organizationCustomDomain)} sx={{ display: 'block' }}>
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

          <StackColumn spacing={1} sx={{ minWidth: 0 }}>
            <SmallIconTypography label="Create teams that regularly work or meet together." />
            <Link component={NextLink} href={getOrganizationTeamAddLink(integratedPlatrform, organizationCustomDomain)} sx={{ display: 'block' }}>
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

          <StackColumn spacing={1} sx={{ minWidth: 0 }}>
            <SmallIconTypography label="Add resources for your locations and teams." />
            <Link component={NextLink} href={getOrganizationAddResourceBaseLink(integratedPlatrform, organizationCustomDomain)} sx={{ display: 'block' }}>
              <Paper sx={{ height: 100, borderRadius: 2, '&:hover': { border: 1, borderColor: emerald } }}>
                <LeadIconTypography
                  label="Add Resources"
                  stackMode="column"
                  startElement={<ResourceIcon fontSize="large" excludeTooltip sx={{ color: emerald }} />}
                  sx={{ alignItems: 'center', justifyContent: 'center', height: '100%' }}
                />
              </Paper>
            </Link>
          </StackColumn>

          <StackColumn spacing={1} sx={{ minWidth: 0 }}>
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
        </Box>
      </Box>

      <InvitePeopleToJoinOrganizationDialog
        isDialogOpen={isInvitePeopleToJoinOrganizationDialogOpen}
        onInviteClicked={handleInvitePeopleToJoinOrganizationClicked}
        onCancel={handleInvitePeopleToJoinOrganizationCancelClicked}
        organizationCustomDomain={organizationCustomDomain}
      />
    </>
  );
};

export default memo(GettingStarted);
