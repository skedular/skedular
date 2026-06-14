import { createWebAppSwitcherModel } from '@/app/app-switcher-config';
import AppSwitcher from '@skedular/ui/app-shell/app-switcher';
import { CustomerAvatar } from '@/components/avatars';
import { NewFeedbackDialog } from '@/components/feedback';
import { BookingIcon, FeedbackIcon, NotificationsIcon, OrganizationIcon, SettingsIcon, SignOutIcon, SystemModeIcon } from '@/components/icons';
import { getCustomerMarketplaceBookingsLink, getNotificationsLink, getSettingsLink, getSignOutReturnToLink, getSpacesAppLink } from '@/components/links';
import type { noOrganizationAppBar_query$key } from '@/queries/__generated__/noOrganizationAppBar_query.graphql';
import DarkModeIcon from '@mui/icons-material/DarkMode';
import LightModeIcon from '@mui/icons-material/LightMode';
import MuiAppBar from '@mui/material/AppBar';
import Badge from '@mui/material/Badge';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Toolbar from '@mui/material/Toolbar';
import Box from '@mui/system/Box';
import { getCustomerFullName, localNow, PaletteModeContext, SelectedPaletteModeContext, toLongDateTime, UpdatePaletteModeContext, useIntegratedPlatform } from '@skedular/shared';
import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, PushToRight, SmallIconTypography, StackColumn } from '@skedular/ui';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import Image from 'next/image';
import NextLink from 'next/link';
import { memo, useContext, useMemo, useState } from 'react';
import { graphql, useFragment } from 'react-relay';
import { useInterval } from 'usehooks-ts';

type Props = {
  rootDataRelay: noOrganizationAppBar_query$key;
  showLogo?: boolean;
};

const NoOrganizationAppBar = ({ rootDataRelay, showLogo }: Props) => {
  const rootData = useFragment<noOrganizationAppBar_query$key>(
    graphql`
      fragment noOrganizationAppBar_query on Query {
        me {
          id
          email
          emails
          givenName
          middleName
          familyName
          photoUrl
        }
        pendingOrganizationInvitationsCount
        pendingTeamInvitationsCount
        ...newFeedbackDialog_query
      }
    `,
    rootDataRelay,
  );

  const { integratedPlatform } = useIntegratedPlatform();
  const { signOut } = useAuth();
  const [currentTime, setCurrentTime] = useState(localNow());
  const selectedThemeMode = useContext(SelectedPaletteModeContext);
  const paletteMode = useContext(PaletteModeContext);
  const updatePaletteMode = useContext(UpdatePaletteModeContext);
  const [themeMenuAnchorEl, setThemeMenuAnchorEl] = useState<null | HTMLElement>(null);
  const [profileOpenAnchorEl, setProfileOpenAnchorEl] = useState<null | HTMLElement>(null);
  const [submitFeedbackDialogOpen, setSubmitFeedbackDialogOpen] = useState(false);
  const appSwitcher = useMemo(() => createWebAppSwitcherModel({ logConfiguration: false }), []);

  useInterval(() => setCurrentTime(localNow()), 1000);

  const handleProfileMenuOpenClick = (event: React.MouseEvent<HTMLElement>) => {
    setProfileOpenAnchorEl(event.currentTarget);
  };

  const handleProfileMenuCloseClick = () => {
    setProfileOpenAnchorEl(null);
  };

  const handleSignOutClick = async () => {
    setProfileOpenAnchorEl(null);
    await signOut({ returnTo: getSignOutReturnToLink() });
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

  const handleThemeMenuOpenClick = (event: React.MouseEvent<HTMLElement>) => {
    setThemeMenuAnchorEl(event.currentTarget);
  };

  const handleThemeMenuCloseClick = () => {
    setThemeMenuAnchorEl(null);
  };

  const handleThemeModeSelected = (mode: 'light' | 'dark' | 'system') => {
    updatePaletteMode(mode);
    handleThemeMenuCloseClick();
  };

  const customerName = getCustomerFullName({
    name: null,
    givenName: rootData.me?.givenName,
    middleName: rootData.me?.middleName,
    familyName: rootData.me?.familyName,
  });

  const settingsLink = getSettingsLink(integratedPlatform);
  const bookingsLink = getCustomerMarketplaceBookingsLink(integratedPlatform);
  const spacesAppLink = getSpacesAppLink();
  const notificationsLink = getNotificationsLink(integratedPlatform);
  const pendingInvitationsCount = rootData.pendingOrganizationInvitationsCount + rootData.pendingTeamInvitationsCount;
  const selectedThemeIcon =
    selectedThemeMode === 'light' ? <LightModeIcon fontSize="small" /> : selectedThemeMode === 'dark' ? <DarkModeIcon fontSize="small" /> : <SystemModeIcon fontSize="small" />;
  const logoUrl = paletteMode === 'dark' ? '/images/skedular-logo-inverse.svg' : '/images/skedular-logo-primary.svg';
  const originalLogoWidth = 779;
  const originalLogoHeight = 163;
  const logoWidth = 230;
  const logoHeight = (originalLogoHeight * logoWidth) / originalLogoWidth;

  return (
    <>
      <MuiAppBar position="sticky" className="app-bar">
        <Toolbar
          sx={{
            backgroundColor: (theme) => theme.palette.background.paper,
            borderBottom: paletteMode === 'dark' ? 1 : undefined,
            borderColor: (theme) => theme.palette.divider,
          }}
        >
          {showLogo && (
            <Box sx={{ display: 'flex', alignItems: 'center', mr: 2 }}>
              <Image src={logoUrl} width={logoWidth} height={logoHeight} unoptimized alt="Skedular" />
            </Box>
          )}
          {showLogo && <Divider orientation="vertical" flexItem sx={{ mr: 2 }} />}

          <BodyIconTypography label={`Welcome ${customerName}`} sx={{ display: { xs: 'none', sm: 'none', md: 'block' } }} />

          <PushToRight />

          <BodyIconTypography label={toLongDateTime(currentTime)} sx={{ display: { xs: 'none', sm: 'none', md: 'block' }, paddingRight: 2 }} />
          <Divider orientation="vertical" flexItem sx={{ display: { xs: 'none', sm: 'block' } }} />

          <Button
            component={NextLink}
            href={spacesAppLink}
            variant="text"
            sx={{
              display: { xs: 'none', md: 'inline-flex' },
              ml: 1,
              minHeight: 40,
              px: 1.25,
              color: (theme) => theme.palette.text.primary,
              fontWeight: 700,
              textTransform: 'none',
              whiteSpace: 'nowrap',
              borderRadius: 0,
              '&:hover': {
                backgroundColor: 'transparent',
                color: (theme) => theme.palette.text.primary,
                textDecoration: 'underline',
              },
            }}
          >
            Become a host
          </Button>

          <IconButton
            component={NextLink}
            href={bookingsLink}
            sx={{
              ml: 1,
              border: 1,
              borderColor: (theme) => theme.palette.divider,
              borderRadius: 3,
              width: 40,
              height: 40,
              color: (theme) => theme.palette.text.primary,
              '&:hover': {
                backgroundColor: (theme) => theme.palette.action.hover,
              },
            }}
          >
            <BookingIcon tip="My bookings" />
          </IconButton>

          <Box sx={{ display: { xs: 'none', md: 'flex' }, alignItems: 'center' }}>
            <IconButton
              onClick={handleThemeMenuOpenClick}
              sx={{
                ml: 1,
                border: 1,
                borderColor: (theme) => theme.palette.divider,
                borderRadius: 3,
                width: 40,
                height: 40,
                color: (theme) => theme.palette.text.primary,
                '&:hover': {
                  backgroundColor: (theme) => theme.palette.action.hover,
                },
              }}
            >
              {selectedThemeIcon}
            </IconButton>

            <Menu
              anchorEl={themeMenuAnchorEl}
              open={Boolean(themeMenuAnchorEl)}
              onClose={handleThemeMenuCloseClick}
              anchorOrigin={{
                vertical: 'bottom',
                horizontal: 'right',
              }}
              transformOrigin={{
                vertical: 'top',
                horizontal: 'right',
              }}
              sx={{ mt: 1 }}
            >
              <MenuItem selected={selectedThemeMode === 'light'} onClick={() => handleThemeModeSelected('light')}>
                <BodyIconTypography startElement={<LightModeIcon fontSize="small" />} label="Light" spacing={2} />
              </MenuItem>
              <MenuItem selected={selectedThemeMode === 'dark'} onClick={() => handleThemeModeSelected('dark')}>
                <BodyIconTypography startElement={<DarkModeIcon fontSize="small" />} label="Dark" spacing={2} />
              </MenuItem>
              <MenuItem selected={selectedThemeMode === 'system'} onClick={() => handleThemeModeSelected('system')}>
                <BodyIconTypography startElement={<SystemModeIcon fontSize="small" />} label="System" spacing={2} />
              </MenuItem>
            </Menu>

            <IconButton color="inherit">
              <Link component={NextLink} href={notificationsLink}>
                {pendingInvitationsCount === 0 && <NotificationsIcon excludeTooltip />}
                {pendingInvitationsCount > 0 && (
                  <Badge badgeContent={pendingInvitationsCount} color="primary">
                    <NotificationsIcon excludeTooltip />
                  </Badge>
                )}
              </Link>
            </IconButton>
          </Box>

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

            <AppSwitcher model={appSwitcher} buttonMode="menu-item" />

            <Divider />

            <MenuItem>
              <Link component={NextLink} href={bookingsLink}>
                <SmallIconTypography startElement={<BookingIcon />} label="My bookings" />
              </Link>
            </MenuItem>

            <MenuItem>
              <Link component={NextLink} href={spacesAppLink}>
                <SmallIconTypography startElement={<OrganizationIcon />} label="Become a host" />
              </Link>
            </MenuItem>

            <MenuItem>
              <Link component={NextLink} href={settingsLink}>
                <SmallIconTypography startElement={<SettingsIcon />} label="Settings" />
              </Link>
            </MenuItem>

            <Divider />

            {/* Notifications & theme — shown in profile menu on mobile only */}
            <Box sx={{ display: { xs: 'block', md: 'none' } }}>
              <MenuItem component={NextLink} href={notificationsLink} onClick={handleProfileMenuCloseClick}>
                {pendingInvitationsCount > 0 ? (
                  <Badge badgeContent={pendingInvitationsCount} color="primary">
                    <SmallIconTypography startElement={<NotificationsIcon excludeTooltip />} label="Notifications" />
                  </Badge>
                ) : (
                  <SmallIconTypography startElement={<NotificationsIcon excludeTooltip />} label="Notifications" />
                )}
              </MenuItem>

              <MenuItem
                selected={selectedThemeMode === 'light'}
                onClick={() => {
                  handleThemeModeSelected('light');
                  handleProfileMenuCloseClick();
                }}
              >
                <SmallIconTypography startElement={<LightModeIcon fontSize="small" />} label="Light mode" />
              </MenuItem>
              <MenuItem
                selected={selectedThemeMode === 'dark'}
                onClick={() => {
                  handleThemeModeSelected('dark');
                  handleProfileMenuCloseClick();
                }}
              >
                <SmallIconTypography startElement={<DarkModeIcon fontSize="small" />} label="Dark mode" />
              </MenuItem>
              <MenuItem
                selected={selectedThemeMode === 'system'}
                onClick={() => {
                  handleThemeModeSelected('system');
                  handleProfileMenuCloseClick();
                }}
              >
                <SmallIconTypography startElement={<SystemModeIcon fontSize="small" />} label="System theme" />
              </MenuItem>

              <Divider />
            </Box>

            <MenuItem onClick={handleSubmitFeedbackClicked}>
              <SmallIconTypography startElement={<FeedbackIcon />} label="Send us feedback" />
            </MenuItem>

            <Divider />

            <MenuItem onClick={async () => await handleSignOutClick()}>
              <SmallIconTypography startElement={<SignOutIcon />} label="Sign out" />
            </MenuItem>
          </Menu>
        </Toolbar>
      </MuiAppBar>

      <NewFeedbackDialog
        rootDataRelay={rootData}
        isDialogOpen={submitFeedbackDialogOpen}
        onSendClicked={handleSubmitFeedbackSendClick}
        onCancel={handleSubmitFeedbackCancelClick}
      />
    </>
  );
};

export default memo(NoOrganizationAppBar);
