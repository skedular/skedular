import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid2';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { CancelIcon, DeskIcon, InviteMemberIcon, LocationIcon, TeamIcon } from '@repo/shared/components/icons';
import { errorNotificationOptions, NotificationContent } from '@repo/shared/components/notification';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { defaultPadding, defaultSpacing } from '@repo/shared/libs/theme';
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
          border: '2px dashed gray',
          paddingTop: defaultPadding,
          paddingLeft: defaultPadding,
          paddingRight: defaultPadding,
          paddingBottom: defaultPadding,
          borderRadius: 4,
          position: 'relative', // Ensure the parent container is relative, this is to make sure the CancelIcon can be placed to the top right corner of the box
        }}
      >
        <Box sx={{ position: 'absolute', top: 1, right: 1 }}>
          <IconButton onClick={handleDismissButtonClicked}>
            <CancelIcon fontSize="large" />
          </IconButton>
        </Box>

        <Typography variant="h5">Getting started</Typography>

        <Grid container spacing={defaultSpacing} sx={{ alignItems: 'center' }}>
          <Grid>
            <Stack direction="column" spacing={1} sx={{ width: 250 }}>
              <Typography variant="body1">Let&apos;s start by setting up the organization&apos;s first location.</Typography>
              <Link href={getLocationAddLink(organizationId)}>
                <Paper elevation={0} sx={{ height: 100, borderRadius: 2 }}>
                  <Stack direction="column" spacing={1} sx={{ alignItems: 'center', justifyContent: 'center', height: '100%' }}>
                    <LocationIcon fontSize="large" excludeTooltip />
                    <Typography variant="h6">Create Location</Typography>
                  </Stack>
                </Paper>
              </Link>
            </Stack>
          </Grid>

          <Grid>
            <Stack direction="column" spacing={1} sx={{ width: 250 }}>
              <Typography variant="body1">Create teams that regularly work or meet together.</Typography>
              <Link href={getTeamAddLink(organizationId)}>
                <Paper elevation={0} sx={{ height: 100, borderRadius: 2 }}>
                  <Stack direction="column" spacing={1} sx={{ alignItems: 'center', justifyContent: 'center', height: '100%' }}>
                    <TeamIcon fontSize="large" excludeTooltip />
                    <Typography variant="h6">Create Team</Typography>
                  </Stack>
                </Paper>
              </Link>
            </Stack>
          </Grid>

          <Grid>
            <Stack direction="column" spacing={1} sx={{ width: 250 }}>
              <Typography variant="body1">Add desks and zones for your locations and teams.</Typography>
              <Paper elevation={0} sx={{ height: 100, borderRadius: 2 }}>
                <Stack direction="column" spacing={1} sx={{ alignItems: 'center', justifyContent: 'center', height: '100%' }}>
                  <DeskIcon fontSize="large" excludeTooltip />
                  <Typography variant="h6">Add Desks</Typography>
                </Stack>
              </Paper>
            </Stack>
          </Grid>

          <Grid>
            <Stack direction="column" spacing={1} sx={{ width: 250 }}>
              <Typography variant="body1">Invite your team members to your organization and start booking!</Typography>
              <Paper elevation={0} sx={{ height: 100, borderRadius: 2 }}>
                <Stack direction="column" spacing={1} sx={{ alignItems: 'center', justifyContent: 'center', height: '100%' }}>
                  <InviteMemberIcon fontSize="large" />
                  <Typography variant="h6">Invite Teammates</Typography>
                </Stack>
              </Paper>
            </Stack>
          </Grid>
        </Grid>
      </Box>
    </Box>
  );
};

export default memo(GettingStarted);
