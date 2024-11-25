import { NewFeedbackDialog } from '@/components/feedback';
import type { oldAppBar_query$key } from '@/queries/__generated__/oldAppBar_query.graphql';
import DarkModeIcon from '@mui/icons-material/DarkMode';
import LightModeIcon from '@mui/icons-material/LightMode';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { FeedbackIcon, LogoutIcon, NotificationsIcon, SettingsIcon, ToggleOffIcon, ToggleOnIcon } from '@repo/shared/components/icons';
import { PaletteModeContext, SwitchToModernUIContext, UpdatePaletteModeContext, UpdateSwitchToModernUIContext } from '@repo/shared/libs/providers';
import { getCustomerFullName, localNow, toLongDateTime } from '@repo/shared/libs/utils';
import { signOut } from 'next-auth/react';
import NextLink from 'next/link';
import { memo, useContext, useState } from 'react';
import { graphql, useFragment } from 'react-relay';
import { useInterval } from 'usehooks-ts';

type Props = {
  rootDataRelay: oldAppBar_query$key;
  onReloadRequired: () => void;
};

const OldAppBar = ({ rootDataRelay }: Props) => {
  const rootData = useFragment<oldAppBar_query$key>(
    graphql`
      fragment oldAppBar_query on Query {
        me {
          email {
            email
            verified
          }
          givenName
          middleName
          familyName
          photoUrl
        }
        ...newFeedbackDialog_query
      }
    `,
    rootDataRelay,
  );

  const [currentTime, setCurrentTime] = useState(localNow());
  const paletteMode = useContext(PaletteModeContext);
  const updatePaletteMode = useContext(UpdatePaletteModeContext);
  const switchToModernUI = useContext(SwitchToModernUIContext);
  const UpdateSwitchToModernUI = useContext(UpdateSwitchToModernUIContext);
  const [profileOpenAnchorEl, setProfileOpenAnchorEl] = useState<null | HTMLElement>(null);
  const [submitFeedbackDialogOpen, setSubmitFeedbackDialogOpen] = useState(false);

  useInterval(() => setCurrentTime(localNow()), 1000);

  const handleProfileMenuOpenClick = (event: React.MouseEvent<HTMLElement>) => {
    setProfileOpenAnchorEl(event.currentTarget);
  };

  const handleProfileMenuCloseClick = () => {
    setProfileOpenAnchorEl(null);
  };

  const handleSignOutClick = () => {
    setProfileOpenAnchorEl(null);
    signOut();
  };

  const handleSubmitFeedbackClicked = () => {
    setProfileOpenAnchorEl(null);
    setSubmitFeedbackDialogOpen(true);
  };

  const handleSubmitFeedbackSendClick = () => {
    setSubmitFeedbackDialogOpen(false);
  };

  const handleSubmitFeedbackCancelClick = () => {
    setSubmitFeedbackDialogOpen(false);
  };

  const handleDarkThemeClicked = () => {
    updatePaletteMode('dark');
  };

  const handleLightThemeClicked = () => {
    updatePaletteMode('light');
  };

  const handleModernUIClicked = () => {
    UpdateSwitchToModernUI(true);
  };

  const handleClassicUIClicked = () => {
    UpdateSwitchToModernUI(false);
  };

  const customerName = getCustomerFullName({
    name: null,
    givenName: rootData.me?.givenName,
    middleName: rootData.me?.middleName,
    familyName: rootData.me?.familyName,
  });

  return (
    <>
      <Stack
        direction="row"
        sx={{ alignItems: 'center', justifyContent: 'space-between', width: '100%', paddingLeft: 1, paddingRight: 1, flexWrap: 'wrap' }}
      >
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <Typography variant="h6" sx={{ display: { xs: 'none', sm: 'block' } }}>{`Welcome ${customerName}`}</Typography>
        </Stack>

        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <Typography variant="h6" sx={{ display: { xs: 'none', sm: 'block' } }}>
            {`${toLongDateTime(currentTime)}`}
          </Typography>
          <Divider orientation="vertical" flexItem />

          <IconButton sx={{ ml: 1 }}>
            <NotificationsIcon />
          </IconButton>

          <IconButton onClick={handleProfileMenuOpenClick}>
            <CustomerAvatar
              name={{
                name: null,
                givenName: rootData.me?.givenName,
                middleName: rootData.me?.middleName,
                familyName: rootData.me?.familyName,
              }}
              photo={{
                url: rootData.me?.photoUrl,
              }}
            />
          </IconButton>

          <Menu
            sx={{ mt: 4 }}
            anchorEl={profileOpenAnchorEl}
            anchorOrigin={{
              vertical: 'top',
              horizontal: 'right',
            }}
            keepMounted
            transformOrigin={{
              vertical: 'top',
              horizontal: 'right',
            }}
            open={Boolean(profileOpenAnchorEl)}
            onClose={handleProfileMenuCloseClick}
          >
            <MenuItem>
              <Stack direction="column">
                <Stack direction="column">
                  <Typography variant="h6">{customerName}</Typography>
                  {rootData.me?.email && <Typography variant="body1">{rootData.me?.email.email}</Typography>}
                </Stack>
              </Stack>
            </MenuItem>

            <Divider />

            <MenuItem>
              <Link component={NextLink} href="/settings" color="inherit">
                <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                  <SettingsIcon fontSize="medium" />
                  <Typography textAlign="center">Settings</Typography>
                </Stack>
              </Link>
            </MenuItem>

            {paletteMode === 'dark' && (
              <MenuItem onClick={handleLightThemeClicked}>
                <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                  <DarkModeIcon fontSize="medium" />
                  <Typography textAlign="center">Dark Mode</Typography>
                </Stack>
              </MenuItem>
            )}

            {paletteMode === 'light' && (
              <MenuItem onClick={handleDarkThemeClicked}>
                <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                  <LightModeIcon fontSize="medium" />
                  <Typography textAlign="center">Light Mode</Typography>
                </Stack>
              </MenuItem>
            )}

            {!switchToModernUI && (
              <MenuItem onClick={handleModernUIClicked}>
                <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                  <ToggleOffIcon fontSize="medium" />
                  <Typography textAlign="center">Switch to modern UI</Typography>
                </Stack>
              </MenuItem>
            )}

            {switchToModernUI && (
              <MenuItem onClick={handleClassicUIClicked}>
                <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                  <ToggleOnIcon fontSize="medium" />
                  <Typography textAlign="center">Switch to classic UI</Typography>
                </Stack>
              </MenuItem>
            )}

            <Divider />

            <MenuItem onClick={handleSubmitFeedbackClicked}>
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                <FeedbackIcon fontSize="medium" />
                <Typography textAlign="center">Send us feedback</Typography>
              </Stack>
            </MenuItem>

            <Divider />

            <MenuItem onClick={handleSignOutClick}>
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                <LogoutIcon fontSize="medium" />
                <Typography textAlign="center">Sign out</Typography>
              </Stack>
            </MenuItem>
          </Menu>
        </Stack>
      </Stack>

      <NewFeedbackDialog
        rootDataRelay={rootData}
        isDialogOpen={submitFeedbackDialogOpen}
        onSendClicked={handleSubmitFeedbackSendClick}
        onCancelClicked={handleSubmitFeedbackCancelClick}
      />
    </>
  );
};

export default memo(OldAppBar);
