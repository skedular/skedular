import { CustomerAvatar, OrganizationAvatar } from '@/components/avatars';
import { NewFeedbackDialog } from '@/components/feedback';
import {
  AddIcon,
  BillingAndPaymentIcon,
  ClaimOwnership,
  FeedbackIcon,
  HamburgerMenuIcon,
  NotificationsIcon,
  OrganizationIcon,
  SettingsIcon,
  SignOutIcon,
  SystemModeIcon,
} from '@/components/icons';
import {
  getBillingAndPaymentLink,
  getNotificationsLink,
  getOrganizationBaseLink,
  getOrganizationLocationsBaseLink,
  getOrganizationSetupLink,
  getSettingsLink,
  getSignOutReturnToLink,
} from '@/components/links';
import { ClaimLocationOwnershipDialog } from '@/components/location';
import { MobileLeftSideNavigationMenu } from '@/components/navigationMenu';
import type { appBar_query$key } from '@/queries/__generated__/appBar_query.graphql';
import DarkModeIcon from '@mui/icons-material/DarkMode';
import LightModeIcon from '@mui/icons-material/LightMode';
import MuiAppBar from '@mui/material/AppBar';
import Badge from '@mui/material/Badge';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import type { SelectChangeEvent } from '@mui/material/Select';
import Select from '@mui/material/Select';
import Toolbar from '@mui/material/Toolbar';
import Box from '@mui/system/Box';
import {
  getCustomerFullName,
  localNow,
  PaletteModeContext,
  SelectedPaletteModeContext,
  toLongDateTime,
  UpdatePaletteModeContext,
  useIntegratedPlatrform,
  useKnownParams,
} from '@skedular/shared';
import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, PushToRight, SmallIconTypography, StackColumn, StackRow } from '@skedular/ui';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import type { JSX } from 'react';
import { memo, useContext, useState } from 'react';
import { graphql, useFragment } from 'react-relay';
import { useInterval } from 'usehooks-ts';

type Props = {
  rootDataRelay: appBar_query$key;
  hideOrganizationSelector?: boolean;
  hideWelcomeMessage?: boolean;
  showBreadcrumps?: boolean;
  breadcrumbs?: React.ReactNode | JSX.Element;
};

const createOrganizationId = '76eZvntIX6YA5FboBJlRk';

const AppBar = ({ rootDataRelay, hideOrganizationSelector, hideWelcomeMessage, showBreadcrumps, breadcrumbs }: Props) => {
  const rootData = useFragment<appBar_query$key>(
    graphql`
      fragment appBar_query on Query {
        me {
          id
          emails
          email
          givenName
          middleName
          familyName
          photoUrl
        }
        myOrganizations(types: [MARKETPLACE, INDIVIDUAL]) {
          uniqueId
          customDomain
          logoUrl
          name
        }
        pendingOrganizationInvitationsCount
        pendingTeamInvitationsCount
        ...mobileLeftSideNavigationMenu_query
        ...newFeedbackDialog_query
      }
    `,
    rootDataRelay,
  );

  const { integratedPlatrform } = useIntegratedPlatrform();
  const { signOut } = useAuth();
  const router = useRouter();
  const { organizationCustomDomain } = useKnownParams();
  const [currentTime, setCurrentTime] = useState(localNow());
  const selectedThemeMode = useContext(SelectedPaletteModeContext);
  const paletteMode = useContext(PaletteModeContext);
  const updatePaletteMode = useContext(UpdatePaletteModeContext);
  const [themeMenuAnchorEl, setThemeMenuAnchorEl] = useState<null | HTMLElement>(null);
  const [profileOpenAnchorEl, setProfileOpenAnchorEl] = useState<null | HTMLElement>(null);
  const [submitFeedbackDialogOpen, setSubmitFeedbackDialogOpen] = useState(false);
  const [claimLocationOwnershipDialogOpen, setClaimLocationOwnershipDialogOpen] = useState(false);
  const [mobileDrawerOpen, setMobileDrawerOpen] = useState(false);

  const [selectedOrganizationId, setSelectedOrganizationId] = useState<string | undefined>(() => {
    if (organizationCustomDomain && rootData.myOrganizations.some((item) => item.customDomain === organizationCustomDomain)) {
      return organizationCustomDomain;
    }

    return undefined;
  });

  useInterval(() => setCurrentTime(localNow()), 1000);

  const handleSelectedOrganizationChange = (event: SelectChangeEvent<unknown>) => {
    const id = event.target.value as string;

    if (id === createOrganizationId) {
      router.push(getOrganizationSetupLink(integratedPlatrform));
    } else {
      setSelectedOrganizationId(id);

      router.push(getOrganizationBaseLink(integratedPlatrform, id));
    }
  };

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

  const handleClaimLocationOwnershipClicked = () => {
    setProfileOpenAnchorEl(null);
    setClaimLocationOwnershipDialogOpen(true);
  };

  const handleClaimLocationOwnershipCompleted = () => {
    setClaimLocationOwnershipDialogOpen(false);

    if (organizationCustomDomain) {
      router.push(getOrganizationLocationsBaseLink(integratedPlatrform, organizationCustomDomain));
    }
  };

  const handleClaimLocationOwnershipCancelled = () => {
    setClaimLocationOwnershipDialogOpen(false);
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
  const notificationsLink = getNotificationsLink(integratedPlatrform);
  const pendingInvitationsCount = rootData.pendingOrganizationInvitationsCount + rootData.pendingTeamInvitationsCount;
  const selectedThemeIcon =
    selectedThemeMode === 'light' ? <LightModeIcon fontSize="small" /> : selectedThemeMode === 'dark' ? <DarkModeIcon fontSize="small" /> : <SystemModeIcon fontSize="small" />;

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
          {!hideOrganizationSelector && (
            <Select
              value={selectedOrganizationId}
              displayEmpty
              onChange={handleSelectedOrganizationChange}
              sx={{
                '& fieldset': {
                  border: 0,
                  borderRight: 0,
                  borderRadius: 0,
                },
              }}
              renderValue={(selectedId) => {
                if (!rootData.myOrganizations) {
                  return (
                    <>
                      <BodyIconTypography
                        label="Please select an organization"
                        sx={{ display: { xs: 'none', sm: 'none', md: 'block' }, overflow: 'hidden', textOverflow: 'ellipsis' }}
                      />
                      <OrganizationIcon tip="Please select an organization" sx={{ display: { xs: 'block', sm: 'block', md: 'none' } }} />
                    </>
                  );
                }

                const selectedItem = rootData.myOrganizations.find((item) => item.customDomain === selectedId);
                if (!selectedItem) {
                  return (
                    <>
                      <BodyIconTypography
                        label="Please select an organization"
                        sx={{ display: { xs: 'none', sm: 'none', md: 'block' }, overflow: 'hidden', textOverflow: 'ellipsis' }}
                      />
                      <OrganizationIcon tip="Please select an organization" sx={{ display: { xs: 'block', sm: 'block', md: 'none' } }} />
                    </>
                  );
                }

                return (
                  <>
                    <Box sx={{ display: { xs: 'none', sm: 'block' }, overflow: 'hidden', textOverflow: 'ellipsis' }}>
                      <LeadIconTypography
                        label={selectedItem.name}
                        sx={{ overflow: 'hidden', textOverflow: 'ellipsis' }}
                        startElement={<OrganizationAvatar name={{ name: selectedItem.name }} photo={{ url: selectedItem.logoUrl }} />}
                      />
                    </Box>

                    <Box sx={{ display: { xs: 'block', sm: 'none' }, overflow: 'hidden', textOverflow: 'ellipsis' }}>
                      <LeadIconTypography label={selectedItem.name} sx={{ width: 150, overflow: 'hidden', textOverflow: 'ellipsis' }} />
                    </Box>
                  </>
                );
              }}
            >
              {rootData.myOrganizations.map((organization) => (
                <MenuItem key={organization.uniqueId} value={organization.customDomain ?? ''}>
                  <StackRow>
                    <OrganizationAvatar name={{ name: organization.name }} photo={{ url: organization.logoUrl }} />
                    <StackColumn spacing={-0.5}>
                      <LeadIconTypography label={organization.name} />
                      <CaptionIconTypography label="Organization" sx={{ display: { xs: 'none', sm: 'block' } }} />
                    </StackColumn>
                  </StackRow>
                </MenuItem>
              ))}

              {rootData.myOrganizations.length !== 0 && <Divider />}

              <MenuItem value={createOrganizationId}>
                <LeadIconTypography label="Create organization" startElement={<AddIcon />} />
              </MenuItem>
            </Select>
          )}

          {!hideWelcomeMessage && (
            <>
              {!hideOrganizationSelector && !hideOrganizationSelector && rootData.myOrganizations.length !== 0 && <Divider orientation="vertical" flexItem />}
              <BodyIconTypography label={`Welcome ${customerName}`} sx={{ display: { xs: 'none', sm: 'none', md: 'block' }, paddingLeft: 2 }} />
            </>
          )}
          {showBreadcrumps && <>{breadcrumbs}</>}

          <PushToRight />
          <BodyIconTypography label={toLongDateTime(currentTime)} sx={{ display: { xs: 'none', sm: 'none', md: 'block' }, paddingRight: 2 }} />
          <Divider orientation="vertical" flexItem sx={{ display: { xs: 'none', sm: 'block' } }} />

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

            <IconButton sx={{ ml: 1, paddingLeft: 2 }} color="inherit">
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

            {selectedOrganizationId && (
              <MenuItem>
                <Link component={NextLink} href={settingsLink}>
                  <SmallIconTypography startElement={<SettingsIcon />} label="Settings" />
                </Link>
              </MenuItem>
            )}

            {selectedOrganizationId && (
              <MenuItem>
                <Link component={NextLink} href={billingAndPaymentLink}>
                  <SmallIconTypography startElement={<BillingAndPaymentIcon />} label="Billing & Payment" />
                </Link>
              </MenuItem>
            )}

            {organizationCustomDomain && (
              <MenuItem onClick={handleClaimLocationOwnershipClicked}>
                <SmallIconTypography startElement={<ClaimOwnership />} label="Claim Location" />
              </MenuItem>
            )}

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

          <MobileLeftSideNavigationMenu rootDataRelay={rootData} open={mobileDrawerOpen} toggleDrawer={toggleMobileDrawerOpen} />
        </Toolbar>
      </MuiAppBar>

      <NewFeedbackDialog
        rootDataRelay={rootData}
        isDialogOpen={submitFeedbackDialogOpen}
        onSendClicked={handleSubmitFeedbackSendClick}
        onCancel={handleSubmitFeedbackCancelClick}
      />

      {organizationCustomDomain && (
        <ClaimLocationOwnershipDialog
          connectionIds={[]}
          isDialogOpen={claimLocationOwnershipDialogOpen}
          onClaimClicked={handleClaimLocationOwnershipCompleted}
          onCancel={handleClaimLocationOwnershipCancelled}
          organizationCustomDomain={organizationCustomDomain}
        />
      )}
    </>
  );
};

export default memo(AppBar);
