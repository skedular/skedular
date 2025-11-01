import { CustomerAvatar, OrganizationAvatar } from '@/components/avatars';
import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, PushToRight, SmallIconTypography, StackColumn, StackRow } from '@/components/commons';
import { NewFeedbackDialog } from '@/components/feedback';
import { AddIcon, BillingAndPaymentIcon, FeedbackIcon, HamburgerMenuIcon, NotificationsIcon, OrganizationIcon, SettingsIcon, SignOutIcon } from '@/components/icons';
import { getBillingAndPaymentLink, getNotificationsLink, getOrganizationBaseLink, getOrganizationSetupLink, getSettingsLink } from '@/components/links';
import { MobileLeftSideNavigationMenu } from '@/components/navigationMenu';
import { PaletteModeContext, UpdatePaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { getCustomerFullName, localNow, toLongDateTime } from '@/libs/utils';
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
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import NextLink from 'next/link';
import { useParams, useRouter } from 'next/navigation';
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
        myOrganizations {
          id
          uniqueAlphanumericName
          logoUrl
          name
          canModify
          canViewAnalytics
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
  const { organizationUniqueAlphanumericName } = useParams();
  const [currentTime, setCurrentTime] = useState(localNow());
  const paletteMode = useContext(PaletteModeContext);
  const updatePaletteMode = useContext(UpdatePaletteModeContext);
  const [profileOpenAnchorEl, setProfileOpenAnchorEl] = useState<null | HTMLElement>(null);
  const [submitFeedbackDialogOpen, setSubmitFeedbackDialogOpen] = useState(false);
  const [mobileDrawerOpen, setMobileDrawerOpen] = useState(false);

  let finalOrganizationUniqueAlphanumericName = '';
  if (typeof organizationUniqueAlphanumericName === 'string') {
    finalOrganizationUniqueAlphanumericName = organizationUniqueAlphanumericName;
  } else if (Array.isArray(organizationUniqueAlphanumericName)) {
    if (typeof organizationUniqueAlphanumericName[0] !== 'undefined') {
      finalOrganizationUniqueAlphanumericName = organizationUniqueAlphanumericName[0];
    }
  }

  const [selectedOrganizationId, setSelectedOrganizationId] = useState<string | undefined>(() => {
    if (finalOrganizationUniqueAlphanumericName && rootData.myOrganizations.some((item) => item.uniqueAlphanumericName === finalOrganizationUniqueAlphanumericName)) {
      return finalOrganizationUniqueAlphanumericName;
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
  const notificationsLink = getNotificationsLink(integratedPlatrform);
  const pendingInvitationsCount = rootData.pendingOrganizationInvitationsCount + rootData.pendingTeamInvitationsCount;

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
          {!hideOrganizationSelector && rootData.myOrganizations.length !== 0 && (
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
                  return <OrganizationIcon tip="Please select an organization" />;
                }

                const selectedItem = rootData.myOrganizations.find((item) => item.uniqueAlphanumericName === selectedId);
                if (!selectedItem) {
                  return <OrganizationIcon tip="Please select an organization" />;
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
                      <LeadIconTypography label={selectedItem.name} sx={{ width: 200, overflow: 'hidden', textOverflow: 'ellipsis' }} />
                    </Box>
                  </>
                );
              }}
            >
              {rootData.myOrganizations.map((organization) => (
                <MenuItem key={organization.id} value={organization.uniqueAlphanumericName ?? ''}>
                  <StackRow>
                    <OrganizationAvatar name={{ name: organization.name }} photo={{ url: organization.logoUrl }} />
                    <StackColumn spacing={-0.5}>
                      <LeadIconTypography label={organization.name} />
                      <CaptionIconTypography label="Organization" sx={{ display: { xs: 'none', sm: 'block' } }} />
                    </StackColumn>
                  </StackRow>
                </MenuItem>
              ))}

              <Divider />

              <MenuItem value={createOrganizationId}>
                <LeadIconTypography label="Create Organization" startElement={<AddIcon />} />
              </MenuItem>
            </Select>
          )}

          {!hideWelcomeMessage && (
            <>
              {!hideOrganizationSelector && !hideOrganizationSelector && rootData.myOrganizations.length !== 0 && <Divider orientation="vertical" flexItem />}
              <BodyIconTypography label={`Welcome ${customerName}`} sx={{ display: { xs: 'none', sm: 'block' }, paddingLeft: 2 }} />
            </>
          )}
          {showBreadcrumps && <>{breadcrumbs}</>}

          <PushToRight />
          <BodyIconTypography label={toLongDateTime(currentTime)} sx={{ display: { xs: 'none', sm: 'block' }, paddingRight: 2 }} />
          <Divider orientation="vertical" flexItem sx={{ display: { xs: 'none', sm: 'block' } }} />

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

            <Divider />

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
    </>
  );
};

export default memo(AppBar);
