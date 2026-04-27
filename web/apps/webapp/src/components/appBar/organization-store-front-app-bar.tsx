import { CustomerAvatar } from '@/components/avatars';
import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, PushToRight, SmallIconTypography, StackColumn } from '@skedular/ui';
import { NewFeedbackDialog } from '@/components/feedback';
import { ArrowDownIcon, BillingAndPaymentIcon, FeedbackIcon, HamburgerMenuIcon, SettingsIcon, SignOutIcon, SystemModeIcon } from '@/components/icons';
import { getBillingAndPaymentLink, getSettingsLink, getSignOutReturnToLink } from '@/components/links';
import { NoOrganizationMobileLeftSideNavigationMenu } from '@/components/navigationMenu';
import { SelectedPaletteModeContext, UpdatePaletteModeContext, useIntegratedPlatrform } from '@skedular/shared';
import { getCustomerFullName } from '@skedular/shared';
import type { organizationStoreFrontAppBar_query$key } from '@/queries/__generated__/organizationStoreFrontAppBar_query.graphql';
import DarkModeIcon from '@mui/icons-material/DarkMode';
import LightModeIcon from '@mui/icons-material/LightMode';
import MuiAppBar from '@mui/material/AppBar';
import Button from '@mui/material/Button';
import Container from '@mui/material/Container';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Toolbar from '@mui/material/Toolbar';
import Box from '@mui/system/Box';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import NextLink from 'next/link';
import { memo, useContext, useState } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: organizationStoreFrontAppBar_query$key;
};

const OrganizationStoreFrontAppBar = ({ rootDataRelay }: Props) => {
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
        organizationPublic(customDomain: $organizationCustomDomain) {
          name
        }
        ...newFeedbackDialog_query
      }
    `,
    rootDataRelay,
  );

  const { integratedPlatrform } = useIntegratedPlatrform();
  const { signOut } = useAuth();
  const selectedThemeMode = useContext(SelectedPaletteModeContext);
  const updatePaletteMode = useContext(UpdatePaletteModeContext);
  const [profileOpenAnchorEl, setProfileOpenAnchorEl] = useState<null | HTMLElement>(null);
  const [themeMenuAnchorEl, setThemeMenuAnchorEl] = useState<null | HTMLElement>(null);
  const [submitFeedbackDialogOpen, setSubmitFeedbackDialogOpen] = useState(false);
  const [mobileDrawerOpen, setMobileDrawerOpen] = useState(false);

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

  const handleOrganizationHomeClick = () => {
    window.location.href = window.location.origin;
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
  const selectedThemeIcon =
    selectedThemeMode === 'light' ? <LightModeIcon fontSize="small" /> : selectedThemeMode === 'dark' ? <DarkModeIcon fontSize="small" /> : <SystemModeIcon fontSize="small" />;

  if (!rootData.organizationPublic) {
    return null;
  }

  return (
    <>
      <MuiAppBar
        position="sticky"
        className="app-bar"
        elevation={0}
        sx={{
          backgroundColor: (theme) => theme.palette.background.default,
          backdropFilter: 'blur(10px)',
          borderBottom: 1,
          borderColor: (theme) => theme.palette.divider,
        }}
      >
        <Container maxWidth="xl">
          <Toolbar
            disableGutters
            sx={{
              minHeight: 'unset !important',
              py: 2.5,
            }}
          >
            <Box
              onClick={handleOrganizationHomeClick}
              sx={{
                cursor: 'pointer',
                borderRadius: 2,
                px: 0.5,
                py: 0.25,
                ml: -0.5,
                transition: 'background-color 120ms ease',
                '&:hover': {
                  backgroundColor: (theme) => theme.palette.action.hover,
                },
              }}
            >
              <LeadIconTypography
                label={rootData.organizationPublic?.name}
                fontWeight={600}
                sx={{
                  letterSpacing: '-0.03em',
                  fontSize: {
                    xs: '1.25rem',
                    sm: '1.5rem',
                  },
                }}
              />
            </Box>

            <PushToRight />

            <Box sx={{ display: { xs: 'none', sm: 'flex' }, alignItems: 'center', gap: 1.5 }}>
              <IconButton
                onClick={handleThemeMenuOpenClick}
                sx={{
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

              <Button
                onClick={handleProfileMenuOpenClick}
                sx={{
                  textTransform: 'none',
                  borderRadius: '24px',
                  px: 3,
                  py: 1,
                  color: (theme) => theme.palette.text.primary,
                  border: 1,
                  borderColor: (theme) => theme.palette.divider,
                  backgroundColor: (theme) => theme.palette.background.paper,
                  fontWeight: 500,
                  fontSize: '0.9375rem',
                  '&:hover': {
                    backgroundColor: (theme) => theme.palette.action.hover,
                    borderColor: (theme) => theme.palette.divider,
                  },
                }}
              >
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
                  sx={{ width: 24, height: 24 }}
                />
                <BodyIconTypography label={customerName || rootData.me?.email || 'Account'} sx={{ ml: 1, mr: 0.5 }} />
                <ArrowDownIcon fontSize="small" />
              </Button>
            </Box>

            <IconButton
              onClick={toggleMobileDrawerOpen(true)}
              sx={{
                display: { xs: 'inline-flex', sm: 'none' },
                ml: 1,
                border: 1,
                borderColor: (theme) => theme.palette.divider,
                borderRadius: 3,
                width: 40,
                height: 40,
              }}
            >
              <HamburgerMenuIcon />
            </IconButton>
          </Toolbar>
        </Container>

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
