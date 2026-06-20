import { PaletteModeContext, getRelayErrorMessage, useIntegratedPlatform } from '@skedular/shared';
import { LeadIconTypography, SectionIconTypography, SmallIconTypography, StackColumn } from '@skedular/ui';
import { CancelIcon, InviteMemberIcon, LocationIcon, ProductIcon } from '@/components/icons';
import { getOrganizationLocationAddMarketplaceLink, getOrganizationLocationsBaseLink, getOrganizationUsersBaseLink } from '@/components/links';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { InvitePeopleToJoinOrganizationDialog } from '@/components/organization/invitePeopleToJoinOrganization';

import { defaultPadding, emerald } from '@skedular/ui';

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

  const { integratedPlatform } = useIntegratedPlatform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [isInvitePeopleToJoinOrganizationDialogOpen, setIsInvitePeopleToJoinOrganizationDialogOpen] = useState(false);

  const handleInvitePeopleClicked = () => {
    setIsInvitePeopleToJoinOrganizationDialogOpen(true);
  };

  const handleInvitePeopleToJoinOrganizationClicked = () => {
    setIsInvitePeopleToJoinOrganizationDialogOpen(false);

    router.push(getOrganizationUsersBaseLink(integratedPlatform, organizationCustomDomain));
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
              xl: 'repeat(3, minmax(0, 1fr))',
            },
            gap: 2,
            alignItems: 'stretch',
            paddingTop: 2,
          }}
        >
          <StackColumn spacing={1} sx={{ minWidth: 0 }}>
            <Box sx={{ minHeight: { sm: 48, xl: 40 } }}>
              <SmallIconTypography label="Let's start by setting up the organization's first location." />
            </Box>
            <Link component={NextLink} href={getOrganizationLocationAddMarketplaceLink(integratedPlatform, organizationCustomDomain)} sx={{ display: 'block' }}>
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
            <Box sx={{ minHeight: { sm: 48, xl: 40 } }}>
              <SmallIconTypography label="Choose a location and define how renters can book it." />
            </Box>
            <Link component={NextLink} href={getOrganizationLocationsBaseLink(integratedPlatform, organizationCustomDomain)} sx={{ display: 'block' }}>
              <Paper sx={{ height: 100, borderRadius: 2, '&:hover': { border: 1, borderColor: emerald } }}>
                <LeadIconTypography
                  label="Create Product"
                  stackMode="column"
                  startElement={<ProductIcon fontSize="large" excludeTooltip sx={{ color: emerald }} />}
                  sx={{ alignItems: 'center', justifyContent: 'center', height: '100%' }}
                />
              </Paper>
            </Link>
          </StackColumn>

          <StackColumn spacing={1} sx={{ minWidth: 0 }}>
            <Box sx={{ minHeight: { sm: 48, xl: 40 } }}>
              <SmallIconTypography label="Invite people to your organization and start booking." />
            </Box>
            <Paper sx={{ height: 100, borderRadius: 2, '&:hover': { border: 1, borderColor: emerald } }} onClick={handleInvitePeopleClicked}>
              <LeadIconTypography
                label="Invite People"
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
