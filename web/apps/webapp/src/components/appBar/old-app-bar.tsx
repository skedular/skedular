import { NewFeedbackDialog } from '@/components/feedback';
import type { oldAppBar_query$key } from '@/queries/__generated__/oldAppBar_query.graphql';
import DarkModeIcon from '@mui/icons-material/DarkMode';
import LightModeIcon from '@mui/icons-material/LightMode';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import {
  BodyIconTypography,
  CaptionIconTypography,
  LeadIconTypography,
  SmallIconTypography,
  StackColumn,
  StackRow,
  StackRowFullWidth,
} from '@repo/shared/components/commons';
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
          email
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
      <StackRowFullWidth
        sx={{
          paddingLeft: 1,
          paddingRight: 1,
          borderBottom: paletteMode === 'dark' ? 1 : undefined,
          borderColor: 'divider',
          backgroundColor: (theme) => theme.palette.background.paper,
        }}
      >
        <BodyIconTypography label={`Welcome ${customerName}`} sx={{ display: { xs: 'none', sm: 'block' }, paddingLeft: 2 }} />

        <StackRow sx={{ alignItems: 'center' }}>
          <BodyIconTypography label={toLongDateTime(currentTime)} sx={{ display: { xs: 'none', sm: 'block' }, paddingRight: 2 }} />
          <Divider orientation="vertical" flexItem />

          <IconButton sx={{ ml: 1, paddingLeft: 2 }} color="inherit">
            <NotificationsIcon excludeTooltip />
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
            sx={{ marginTop: 4 }}
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
            slotProps={{ paper: { sx: { borderRadius: 2, boxShadow: 3 } } }}
          >
            <MenuItem>
              <StackColumn>
                <LeadIconTypography label={customerName} />
                <CaptionIconTypography label={rootData.me?.email} />
              </StackColumn>
            </MenuItem>

            <Divider />

            <MenuItem>
              <Link component={NextLink} href="/settings" color="inherit">
                <SmallIconTypography startElement={<SettingsIcon />} label="Settings" />
              </Link>
            </MenuItem>

            {paletteMode === 'dark' && (
              <MenuItem onClick={handleLightThemeClicked}>
                <SmallIconTypography startElement={<DarkModeIcon />} label="Dark Mode" />
              </MenuItem>
            )}

            {paletteMode === 'light' && (
              <MenuItem onClick={handleDarkThemeClicked}>
                <SmallIconTypography startElement={<LightModeIcon />} label="Light Mode" />
              </MenuItem>
            )}

            {!switchToModernUI && (
              <MenuItem onClick={handleModernUIClicked}>
                <SmallIconTypography startElement={<ToggleOffIcon />} label="Switch to modern UI" />
              </MenuItem>
            )}

            {switchToModernUI && (
              <MenuItem onClick={handleClassicUIClicked}>
                <SmallIconTypography startElement={<ToggleOnIcon />} label="Switch to classic UI" />
              </MenuItem>
            )}

            <Divider />

            <MenuItem onClick={handleSubmitFeedbackClicked}>
              <SmallIconTypography startElement={<FeedbackIcon />} label="Send us feedback" />
            </MenuItem>

            <Divider />

            <MenuItem onClick={handleSignOutClick}>
              <SmallIconTypography startElement={<LogoutIcon />} label="Sign out" />
            </MenuItem>
          </Menu>
        </StackRow>
      </StackRowFullWidth>

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
