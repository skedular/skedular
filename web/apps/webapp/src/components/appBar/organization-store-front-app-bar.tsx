import { CustomerAvatar } from '@/components/avatars';
import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, PushToRight, SmallIconTypography, StackColumn } from '@/components/commons';
import { NewFeedbackDialog } from '@/components/feedback';
import { BillingAndPaymentIcon, FeedbackIcon, HamburgerMenuIcon, SettingsIcon, SignOutIcon } from '@/components/icons';
import { getBillingAndPaymentLink, getSettingsLink } from '@/components/links';
import { NoOrganizationMobileLeftSideNavigationMenu } from '@/components/navigationMenu';
import { PaletteModeContext, UpdatePaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { getCustomerFullName, localNow, toLongDateTime } from '@/libs/utils';
import type { organizationStoreFrontAppBar_query$key } from '@/queries/__generated__/organizationStoreFrontAppBar_query.graphql';
import DarkModeIcon from '@mui/icons-material/DarkMode';
import LightModeIcon from '@mui/icons-material/LightMode';
import MuiAppBar from '@mui/material/AppBar';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Toolbar from '@mui/material/Toolbar';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import NextLink from 'next/link';
import { memo, useContext, useState } from 'react';
import { graphql, useFragment } from 'react-relay';
import { useInterval } from 'usehooks-ts';

type Props = {
  rootDataRelay: organizationStoreFrontAppBar_query$key;
  hideWelcomeMessage?: boolean;
};

const OrganizationStoreFrontAppBar = ({ rootDataRelay, hideWelcomeMessage }: Props) => {
  const rootData = useFragment<organizationStoreFrontAppBar_query$key>(
    graphql`
      fragment organizationStoreFrontAppBar_query on Query {
        me {
          id
          email
          emails
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

  const { integratedPlatrform } = useIntegratedPlatrform();
  const { signOut } = useAuth();
  const [currentTime, setCurrentTime] = useState(localNow());
  const paletteMode = useContext(PaletteModeContext);
  const updatePaletteMode = useContext(UpdatePaletteModeContext);
  const [profileOpenAnchorEl, setProfileOpenAnchorEl] = useState<null | HTMLElement>(null);
  const [submitFeedbackDialogOpen, setSubmitFeedbackDialogOpen] = useState(false);
  const [mobileDrawerOpen, setMobileDrawerOpen] = useState(false);

  useInterval(() => setCurrentTime(localNow()), 1000);

  const handleProfileMenuOpenClick = (event: React.MouseEvent<HTMLElement>) => {
    setProfileOpenAnchorEl(event.currentTarget);
  };

  const handleProfileMenuCloseClick = () => {
    setProfileOpenAnchorEl(null);
  };

  const handleSignOutClick = async () => {
    setProfileOpenAnchorEl(null);
    await signOut();
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

  const toggleMobileDrawerOpen = (newOpen: boolean) => () => {
    setMobileDrawerOpen(newOpen);
  };

  const customerName = getCustomerFullName({
    name: null,
    givenName: rootData.me?.givenName,
    middleName: rootData.me?.middleName,
    familyName: rootData.me?.familyName,
  });

  const settingsLink = getSettingsLink(integratedPlatrform);
  const billingAndPaymentLink = getBillingAndPaymentLink(integratedPlatrform);

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
          {!hideWelcomeMessage && <BodyIconTypography label={`Welcome ${customerName}`} sx={{ display: { xs: 'none', sm: 'none', md: 'block' }, paddingLeft: 2 }} />}

          <PushToRight />
          <BodyIconTypography label={toLongDateTime(currentTime)} sx={{ display: { xs: 'none', sm: 'none', md: 'block' }, paddingRight: 2 }} />
          <Divider orientation="vertical" flexItem sx={{ display: { xs: 'none', sm: 'block' } }} />

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

          <IconButton onClick={toggleMobileDrawerOpen(true)} sx={{ display: { xs: 'block', sm: 'none' } }}>
            <HamburgerMenuIcon />
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
              <Link component={NextLink} href={settingsLink}>
                <SmallIconTypography startElement={<SettingsIcon />} label="Settings" />
              </Link>
            </MenuItem>

            <MenuItem>
              <Link component={NextLink} href={billingAndPaymentLink}>
                <SmallIconTypography startElement={<BillingAndPaymentIcon />} label="Billing & Payment" />
              </Link>
            </MenuItem>
            {paletteMode === 'dark' && (
              <MenuItem onClick={handleLightThemeClicked}>
                <SmallIconTypography startElement={<LightModeIcon />} label="Light Mode" />
              </MenuItem>
            )}

            {paletteMode === 'light' && (
              <MenuItem onClick={handleDarkThemeClicked}>
                <SmallIconTypography startElement={<DarkModeIcon />} label="Dark Mode" />
              </MenuItem>
            )}

            <Divider />

            <MenuItem onClick={handleSubmitFeedbackClicked}>
              <SmallIconTypography startElement={<FeedbackIcon />} label="Send us feedback" />
            </MenuItem>

            <Divider />

            <MenuItem onClick={async () => await handleSignOutClick()}>
              <SmallIconTypography startElement={<SignOutIcon />} label="Sign out" />
            </MenuItem>
          </Menu>

          <NoOrganizationMobileLeftSideNavigationMenu open={mobileDrawerOpen} toggleDrawer={toggleMobileDrawerOpen} />
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

export default memo(OrganizationStoreFrontAppBar);
