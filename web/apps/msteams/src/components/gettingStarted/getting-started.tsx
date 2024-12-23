import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid2';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Paper from '@mui/material/Paper';
import { GridContainer, LeadIconTypography, SectionIconTypography, SmallIconTypography, StackColumn } from '@repo/shared/components/commons';
import { CancelIcon, DeskIcon, InviteMemberIcon, LocationIcon, TeamIcon } from '@repo/shared/components/icons';
import { errorNotificationOptions, NotificationContent } from '@repo/shared/components/notification';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { defaultPadding, emerald } from '@repo/shared/libs/theme';
import { joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { getLocationAddLink } from 'components/location';
import { getTeamAddLink } from 'components/team';
import { nanoid } from 'nanoid';
import { memo, useContext } from 'react';
import { useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import type { gettingStarted_completeOrganizationMemberOnboardingMutation } from './__generated__/gettingStarted_completeOrganizationMemberOnboardingMutation.graphql';
import type { gettingStarted_query$key } from './__generated__/gettingStarted_query.graphql';

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

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;

  const handleDismissButtonClicked = () => {
    commitCompleteOrganizationMemberOnboarding({
      variables: {
        input: {
          clientMutationId: nanoid(),
          organizationId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(
            <NotificationContent content={`Failed to dismiss organization onboarding. Error: ${joinErrors(errors)}.`} />,
            errorNotificationOptions,
          );
        }

        onReloadRequired();
      },
      onError: (error) => {
        themedToast(
          <NotificationContent content={`Failed to dismiss organization onboarding. Error: ${error.message}}.`} />,
          errorNotificationOptions,
        );
      },
    });
  };

  if (!rootData.organization || rootData.organization.isMyOnboardingDone) {
    return <></>;
  }

  return (
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

        <GridContainer spacing={{ xs: 1, sm: 1, md: 1, lg: 15 }} sx={{ alignItems: 'center', paddingTop: 2 }}>
          <Grid>
            <StackColumn sx={{ width: 250 }}>
              <SmallIconTypography label="Let's start by setting up the organization's first location." />
              <Link href={getLocationAddLink(organizationId)}>
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
              <Link href={getTeamAddLink(organizationId)}>
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
              <SmallIconTypography label="Add desks and zones for your locations and teams." />
              <Paper sx={{ height: 100, borderRadius: 2, '&:hover': { border: 1, borderColor: emerald } }}>
                <LeadIconTypography
                  label="Add Desks"
                  stackMode="column"
                  startElement={<DeskIcon fontSize="large" excludeTooltip sx={{ color: emerald }} />}
                  sx={{ alignItems: 'center', justifyContent: 'center', height: '100%' }}
                />
              </Paper>
            </StackColumn>
          </Grid>

          <Grid>
            <StackColumn sx={{ width: 250 }}>
              <SmallIconTypography label="Invite your team members to your organization and start booking!" />
              <Paper sx={{ height: 100, borderRadius: 2, '&:hover': { border: 1, borderColor: emerald } }}>
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
  );
};

export default memo(GettingStarted);
