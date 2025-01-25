import { IconButton } from '@mui/material';
import Box from '@mui/material/Box';
import Link from '@mui/material/Link';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import { BodyIconTypography } from '@repo/shared/components/commons';
import { CollpaseDrawerIcon, HomeIcon, LocationIcon, NotificationsIcon, SettingsIcon, TeamIcon } from '@repo/shared/components/icons';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { getSelectedListItemBorderRadius, sandstone, secondDrawerCollapsedDrawerWidth, secondDrawerExpandedDrawerWidth, selectedListItemPaddings } from '@repo/shared/libs/theme';
import { memo, useContext } from 'react';
import { useLocation, useParams } from 'react-router-dom';

type Props = {
  collapsed?: boolean;
  toggleCollapse?: (collapsed: boolean) => void;
  enableCollapseButton?: boolean;
  hideIcons?: boolean;
};

const LeftSideNavigationMenuContent = ({ collapsed, enableCollapseButton, toggleCollapse, hideIcons }: Props) => {
  const location = useLocation();
  const pathname = location.pathname;
  const paletteMode = useContext(PaletteModeContext);
  const maxWidth = collapsed ? secondDrawerCollapsedDrawerWidth : secondDrawerExpandedDrawerWidth;
  const logoUrl =
    paletteMode === 'dark'
      ? collapsed
        ? '/images/skedular-icon-inverse.svg'
        : '/images/skedular-logo-inverse.svg'
      : collapsed
        ? '/images/skedular-icon-primary.svg'
        : '/images/skedular-logo-primary.svg';
  const originalWidth = 779;
  const originalHeight = 163;
  const widthPercentage = ((maxWidth - 70) * 100) / originalWidth;
  const heightPercentage = ((maxWidth - 30) * 100) / originalWidth;
  const width = collapsed ? 30 : (originalWidth * widthPercentage) / 100;
  const height = collapsed ? 30 : (originalHeight * heightPercentage) / 100;
  const styles = {
    width: maxWidth - 30,
    marginLeft: 2,
    marginRight: 2,
    transition: 'border-radius 0.3s ease, width 0.3s ease',
    '&:hover': {
      borderRadius: 4,
      width: maxWidth - 30,
      marginLeft: 2,
      marginRight: 2,
      transition: 'none',
    },
    '&.Mui-selected': {
      width: maxWidth - 30,
      backgroundColor: sandstone,
      '&:hover': {
        backgroundColor: sandstone,
      },
    },
    ...selectedListItemPaddings,
  };

  const { organizationId } = useParams();
  let finalOrganizationId = '';

  if (typeof organizationId === 'string') {
    finalOrganizationId = organizationId;
  } else if (Array.isArray(organizationId)) {
    if (typeof organizationId[0] === 'undefined') {
      throw new Error('organizationId is required');
    }

    finalOrganizationId = organizationId[0];
  } else {
    throw new Error('organizationId is required');
  }

  const handleCollpaseClicked = () => {
    if (toggleCollapse) {
      toggleCollapse(true);
    }
  };

  const handleExpandClicked = () => {
    if (toggleCollapse) {
      toggleCollapse(false);
    }
  };

  return (
    <>
      {enableCollapseButton && !collapsed && (
        <IconButton
          sx={{
            position: 'absolute',
            top: 0,
            right: 0,
            transform: 'translate(0%, 80%)',
            zIndex: (theme) => theme.zIndex.drawer + 1,
          }}
          size="small"
          onClick={handleCollpaseClicked}
        >
          <CollpaseDrawerIcon fontSize="small" />
        </IconButton>
      )}

      <List>
        <ListItem
          disablePadding
          sx={{ width: collapsed ? undefined : maxWidth - 30, justifyContent: 'center', marginLeft: 0, paddingBottom: { xs: 1, sm: 1, md: 5 } }}
          onClick={handleExpandClicked}
        >
          <Box component="img" sx={{ width, height }} alt="Skedular" src={logoUrl} />
        </ListItem>

        <ListItem disablePadding>
          <Link href={`/organizations/${finalOrganizationId}`}>
            <ListItemButton
              selected={pathname === `/organizations/${finalOrganizationId}`}
              sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathname === `/organizations/${finalOrganizationId}`) }}
            >
              {collapsed && (
                <BodyIconTypography
                  startElement={!hideIcons && <HomeIcon color="inherit" />}
                  invertDefaultColor={pathname === `/organizations/${finalOrganizationId}` && paletteMode === 'dark'}
                />
              )}
              {!collapsed && (
                <BodyIconTypography
                  label="Home"
                  startElement={!hideIcons && <HomeIcon excludeTooltip color="inherit" />}
                  spacing={3}
                  invertDefaultColor={pathname === `/organizations/${finalOrganizationId}` && paletteMode === 'dark'}
                />
              )}
            </ListItemButton>
          </Link>
        </ListItem>

        <ListItem disablePadding>
          <Link href={`/organizations/${finalOrganizationId}/locations`}>
            <ListItemButton
              selected={pathname.startsWith(`/organizations/${finalOrganizationId}/locations`)}
              sx={{
                ...styles,
                borderRadius: getSelectedListItemBorderRadius(pathname.startsWith(`/organizations/${finalOrganizationId}/locations`)),
              }}
            >
              {collapsed && (
                <BodyIconTypography
                  startElement={!hideIcons && <LocationIcon color="inherit" />}
                  invertDefaultColor={pathname.startsWith(`/organizations/${finalOrganizationId}/locations`) && paletteMode === 'dark'}
                />
              )}
              {!collapsed && (
                <BodyIconTypography
                  label="Locations"
                  startElement={!hideIcons && <LocationIcon excludeTooltip color="inherit" />}
                  spacing={3}
                  invertDefaultColor={pathname.startsWith(`/organizations/${finalOrganizationId}/locations`) && paletteMode === 'dark'}
                />
              )}
            </ListItemButton>
          </Link>
        </ListItem>

        <ListItem disablePadding>
          <Link href={`/organizations/${finalOrganizationId}/teams`}>
            <ListItemButton
              selected={pathname.startsWith(`/organizations/${finalOrganizationId}/teams`)}
              sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathname.startsWith(`/organizations/${finalOrganizationId}/teams`)) }}
            >
              {collapsed && (
                <BodyIconTypography
                  startElement={!hideIcons && <TeamIcon color="inherit" />}
                  invertDefaultColor={pathname.startsWith(`/organizations/${finalOrganizationId}/teams`) && paletteMode === 'dark'}
                />
              )}
              {!collapsed && (
                <BodyIconTypography
                  label="Teams"
                  startElement={!hideIcons && <TeamIcon excludeTooltip color="inherit" />}
                  spacing={3}
                  invertDefaultColor={pathname.startsWith(`/organizations/${finalOrganizationId}/teams`) && paletteMode === 'dark'}
                />
              )}
            </ListItemButton>
          </Link>
        </ListItem>

        <ListItem disablePadding>
          <Link href={`/${finalOrganizationId}/notifications`}>
            <ListItemButton
              selected={pathname.startsWith(`/${finalOrganizationId}/notifications`)}
              sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathname.startsWith(`/${finalOrganizationId}/notifications`)) }}
            >
              {collapsed && (
                <BodyIconTypography
                  startElement={!hideIcons && <NotificationsIcon color="inherit" />}
                  invertDefaultColor={pathname.startsWith(`/${finalOrganizationId}/notifications`) && paletteMode === 'dark'}
                />
              )}
              {!collapsed && (
                <BodyIconTypography
                  label="Notifications"
                  startElement={!hideIcons && <NotificationsIcon excludeTooltip color="inherit" />}
                  spacing={3}
                  invertDefaultColor={pathname.startsWith(`/${finalOrganizationId}/notifications`) && paletteMode === 'dark'}
                />
              )}
            </ListItemButton>
          </Link>
        </ListItem>

        <ListItem disablePadding>
          <Link href={`/${finalOrganizationId}/settings`}>
            <ListItemButton
              selected={pathname.startsWith(`/${finalOrganizationId}/settings`)}
              sx={{ ...styles, borderRadius: getSelectedListItemBorderRadius(pathname.startsWith(`/${finalOrganizationId}/settings`)) }}
            >
              {collapsed && (
                <BodyIconTypography
                  startElement={!hideIcons && <SettingsIcon color="inherit" />}
                  invertDefaultColor={pathname.startsWith(`/${finalOrganizationId}/settings`) && paletteMode === 'dark'}
                />
              )}
              {!collapsed && (
                <BodyIconTypography
                  label="Settings"
                  startElement={!hideIcons && <SettingsIcon excludeTooltip color="inherit" />}
                  spacing={3}
                  invertDefaultColor={pathname.startsWith(`/${finalOrganizationId}/settings`) && paletteMode === 'dark'}
                />
              )}
            </ListItemButton>
          </Link>
        </ListItem>
      </List>
    </>
  );
};

export default memo(LeftSideNavigationMenuContent);
