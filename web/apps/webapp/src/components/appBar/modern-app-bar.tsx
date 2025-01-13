import { NewFeedbackDialog } from '@/components/feedback';
import { MobileLeftSideNavigationMenu } from '@/components/navigationMenu';
import { getOrganizationAddLink, getOrganizationBaseLink } from '@/components/organization/organization-link';
import { SelectedOrganizationContext, UpdateSelectedOrganizationContext } from '@/libs/providers';
import type { modernAppBar_query$key } from '@/queries/__generated__/modernAppBar_query.graphql';
import DarkModeIcon from '@mui/icons-material/DarkMode';
import LightModeIcon from '@mui/icons-material/LightMode';
import AppBar from '@mui/material/AppBar';
import Divider from '@mui/material/Divider';
import FormControl from '@mui/material/FormControl';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import type { SelectChangeEvent } from '@mui/material/Select';
import Select from '@mui/material/Select';
import Toolbar from '@mui/material/Toolbar';
import Box from '@mui/system/Box';
import { CustomerAvatar, OrganizationAvatar } from '@repo/shared/components/avatars';
import {
  BodyIconTypography,
  CaptionIconTypography,
  LeadIconTypography,
  PushToRight,
  SmallIconTypography,
  StackColumn,
  StackRow,
} from '@repo/shared/components/commons';
import {
  AddIcon,
  FeedbackIcon,
  HamburgerMenuIcon,
  LogoutIcon,
  NotificationsIcon,
  SettingsIcon,
  ToggleOffIcon,
  ToggleOnIcon,
} from '@repo/shared/components/icons';
import { PaletteModeContext, SwitchToModernUIContext, UpdatePaletteModeContext, UpdateSwitchToModernUIContext } from '@repo/shared/libs/providers';
import { getCustomerFullName, localNow, toLongDateTime } from '@repo/shared/libs/utils';
import { signOut } from 'next-auth/react';
import NextLink from 'next/link';
import { useParams, usePathname, useRouter } from 'next/navigation';
import type { JSX } from 'react';
import { memo, useContext, useEffect, useState } from 'react';
import { graphql, useFragment } from 'react-relay';
import { useInterval } from 'usehooks-ts';

type Props = {
  rootDataRelay: modernAppBar_query$key;
  hideOrganizationSelector?: boolean;
  hideWelcomeMessage?: boolean;
  showBreadcrumps?: boolean;
  breadcrumbs?: React.ReactNode | JSX.Element;
};

const createOrganizationId = '76eZvntIX6YA5FboBJlRk';

const ModernAppBar = ({ rootDataRelay, hideOrganizationSelector, hideWelcomeMessage, showBreadcrumps, breadcrumbs }: Props) => {
  const rootData = useFragment<modernAppBar_query$key>(
    graphql`
      fragment modernAppBar_query on Query {
        me {
          email
          givenName
          middleName
          familyName
          photoUrl
        }
        myOrganizations {
          id
          logoUrl
          name
          canModify
          canViewAnalytics
        }
        ...mobileLeftSideNavigationMenu_query
        ...newFeedbackDialog_query
      }
    `,
    rootDataRelay,
  );

  const pathName = usePathname();
  const router = useRouter();
  const { organizationId } = useParams();
  const [currentTime, setCurrentTime] = useState(localNow());
  const selectedOrganization = useContext(SelectedOrganizationContext);
  const updateSelectedOrganization = useContext(UpdateSelectedOrganizationContext);
  const paletteMode = useContext(PaletteModeContext);
  const updatePaletteMode = useContext(UpdatePaletteModeContext);
  const switchToModernUI = useContext(SwitchToModernUIContext);
  const UpdateSwitchToModernUI = useContext(UpdateSwitchToModernUIContext);
  const [profileOpenAnchorEl, setProfileOpenAnchorEl] = useState<null | HTMLElement>(null);
  const [submitFeedbackDialogOpen, setSubmitFeedbackDialogOpen] = useState(false);
  const [mobileDrawerOpen, setMobileDrawerOpen] = useState(false);

  let finalOrganizationId = '';
  if (typeof organizationId === 'string') {
    finalOrganizationId = organizationId;
  } else if (Array.isArray(organizationId)) {
    if (typeof organizationId[0] !== 'undefined') {
      finalOrganizationId = organizationId[0];
    }
  }

  const [selectedOrganizationId, setSelectedOrganizationId] = useState<string | undefined>(() => {
    if (finalOrganizationId && rootData.myOrganizations && rootData.myOrganizations.some((item) => item.id === finalOrganizationId)) {
      return finalOrganizationId;
    }

    if (selectedOrganization && rootData.myOrganizations && rootData.myOrganizations.some((item) => item.id === selectedOrganization)) {
      return selectedOrganization;
    }

    return rootData.myOrganizations && rootData.myOrganizations.length > 0 ? rootData.myOrganizations[0]?.id : undefined;
  });

  useInterval(() => setCurrentTime(localNow()), 1000);

  useEffect(() => {
    if (pathName === getOrganizationAddLink() || finalOrganizationId || !selectedOrganizationId) {
      return;
    }

    router.push(getOrganizationBaseLink(selectedOrganizationId));
  }, [router, finalOrganizationId, selectedOrganizationId, pathName]);

  const handleSelectedOrganizationChange = (event: SelectChangeEvent<unknown>) => {
    const id = event.target.value as string;

    if (id === createOrganizationId) {
      router.push(getOrganizationAddLink());
    } else {
      setSelectedOrganizationId(id);
      updateSelectedOrganization(id);

      router.push(getOrganizationBaseLink(id));
    }
  };

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

  const toggleMobileDrawerOpen = (newOpen: boolean) => () => {
    setMobileDrawerOpen(newOpen);
  };

  if (!rootData.myOrganizations) {
    return <></>;
  }

  const customerName = getCustomerFullName({
    name: null,
    givenName: rootData.me?.givenName,
    middleName: rootData.me?.middleName,
    familyName: rootData.me?.familyName,
  });

  return (
    <>
      <AppBar position="sticky" className="app-bar">
        <Toolbar
          sx={{
            backgroundColor: (theme) => theme.palette.background.paper,
            borderBottom: paletteMode === 'dark' ? 1 : undefined,
            borderColor: (theme) => theme.palette.divider,
          }}
        >
          {!hideOrganizationSelector && rootData.myOrganizations.length !== 0 && (
            <FormControl sx={{ width: { xs: '100%', sm: 300 } }}>
              <Select
                value={selectedOrganizationId}
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
                    return <></>;
                  }

                  const selectedItem = rootData.myOrganizations.find((item) => item.id === selectedId);
                  if (!selectedItem) {
                    return <></>;
                  }

                  return (
                    <StackRow>
                      <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
                        <OrganizationAvatar name={{ name: selectedItem.name }} photo={{ url: selectedItem.logoUrl }} />
                      </Box>
                      <StackColumn spacing={-0.5}>
                        <LeadIconTypography label={selectedItem.name} />
                        <CaptionIconTypography label="Organization" sx={{ display: { xs: 'none', sm: 'block' } }} />
                      </StackColumn>
                    </StackRow>
                  );
                }}
              >
                {rootData.myOrganizations.map((organization) => (
                  <MenuItem key={organization.id} value={organization.id}>
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
            </FormControl>
          )}

          {!hideWelcomeMessage && (
            <>
              {!hideOrganizationSelector && !hideOrganizationSelector && rootData.myOrganizations.length !== 0 && (
                <Divider orientation="vertical" flexItem />
              )}
              <BodyIconTypography label={`Welcome ${customerName}`} sx={{ display: { xs: 'none', sm: 'block' }, paddingLeft: 2 }} />
            </>
          )}
          {showBreadcrumps && <>{breadcrumbs}</>}

          <PushToRight />
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

          <MobileLeftSideNavigationMenu rootDataRelay={rootData} open={mobileDrawerOpen} toggleDrawer={toggleMobileDrawerOpen} />
        </Toolbar>
      </AppBar>

      <NewFeedbackDialog
        rootDataRelay={rootData}
        isDialogOpen={submitFeedbackDialogOpen}
        onSendClicked={handleSubmitFeedbackSendClick}
        onCancel={handleSubmitFeedbackCancelClick}
      />
    </>
  );
};

export default memo(ModernAppBar);
